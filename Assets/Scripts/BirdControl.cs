using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class BirdControl : MonoBehaviour
{
    public Transform transform;
    public Rigidbody body;
    public Animator animator;
    public float curZoom;
    public float maxZoom;
    public float minZoom;
    public float accelerationCounter;
    public Transform camFocus;
    public Transform cam;
    public float personalSensitivity;
    private float lift;
    private float liftoffCounter;
    private bool inAir;
    private int2 curInput;
    private Vector2 camRotation;
    private Vector2 birdRotation;
    private Keyboard kb;
    private Mouse mouse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (kb == null || mouse == null)
        {
            kb = Keyboard.current;
            mouse = Mouse.current;
        }
        else
        {
            liftoffCounter -= Time.deltaTime;
            if (kb.spaceKey.wasPressedThisFrame)
            {
                inAir = true;
                animator.SetInteger("InAir", 1);
                liftoffCounter = 3;
            }
            CamRotation();
        }
    }

    private void FixedUpdate()
    {
        if (kb == null || mouse == null)
        {
            kb = Keyboard.current;
            mouse = Mouse.current;
        }
        else
        {
            curInput = int2.zero;
            if (kb.aKey.isPressed)
                curInput = new int2(-1, 0);
            else if (kb.dKey.isPressed)
                curInput = new int2(1, 0);
            if (kb.wKey.isPressed)
                curInput = new int2(curInput.x, 1);
            else if (kb.sKey.isPressed)
                curInput = new int2(curInput.x, -1);
            if (curInput.x != 0 || curInput.y != 0)
            {
                accelerationCounter += Time.deltaTime;
                if (accelerationCounter > 1) accelerationCounter = 1;
            }
            else
            {
                accelerationCounter -= Time.deltaTime * 2;
                if (accelerationCounter < 0) accelerationCounter = 0;
            }
                Move();
        }
    }

    private void LateUpdate()
    {
        //transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    private void CamRotation()
    {
        if (!kb.tabKey.isPressed)
        {
            camRotation += new Vector2(-mouse.delta.value.y, mouse.delta.value.x) * personalSensitivity;
            if (camRotation.x < -90) camRotation = new Vector2(-90, camRotation.y);
            else if (camRotation.x > 90) camRotation = new Vector2(90, camRotation.y);
            camFocus.rotation = Quaternion.Euler(camRotation);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        curZoom -= mouse.scroll.value.y / 3;
        if (curZoom > maxZoom / 3) curZoom = maxZoom / 3;
        else if (curZoom < minZoom / 3) curZoom = minZoom / 3;
        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, -curZoom);
    }

    private void Move()
    {
        Vector3 inputVelocity = camFocus.transform.rotation * new Vector3(curInput.x, 0, curInput.y);
        float angle = Vector3.Angle(inputVelocity, body.linearVelocity);
        body.linearVelocity += inputVelocity * 20 * (1 + angle * .02f) * Time.fixedDeltaTime;
        float velocity = body.linearVelocity.magnitude;
        if (inAir)
        {
            angle = Vector3.Angle(transform.forward, new Vector3(transform.forward.x, 0, transform.forward.z));
            lift = velocity * .5f * (1 - 1f / 90 * angle);
            if (lift > 9.5f) lift = 9.5f;
            body.linearVelocity += new Vector3(0, lift * Time.fixedDeltaTime, 0);
            animator.SetFloat("Fly", accelerationCounter);
            if (body.linearVelocity.y < 0 && transform.position.y < 2.5f && liftoffCounter < 0)
            {
                inAir = false;
                animator.SetInteger("InAir", 0);
            }
            transform.forward = body.linearVelocity;
        }
        else
        {
            if (body.linearVelocity.sqrMagnitude > .01f)
                transform.forward = new Vector3(body.linearVelocity.x, 0, body.linearVelocity.z).normalized;
            body.linearVelocity *= .9f;
            animator.SetFloat("Walk", accelerationCounter);
        }
    }
}
