using UnityEngine;
using UnityEngine.InputSystem;

public class AnimTesting : MonoBehaviour
{
    public Animator animator;

    Keyboard kb;
    
    void Start()
    {
        
    }

    void Update()
    {
        // keyboard assignment doesn't tend to pick up in the first frames
        if (kb == null) kb = Keyboard.current;
        else
        {
            if (kb.kKey.wasPressedThisFrame)
                animator.SetInteger("Flip", 1);
        }
    }

    public void EndFlip()
    {
        animator.SetInteger("Flip", 0);
    }
}
