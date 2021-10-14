using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    #region Fields
    private Animator animator;
    #endregion

    #region Properties

    public bool IsOpen { get { return animator.GetBool("isOpen"); } }

    #endregion

    #region Unity Methods

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    #endregion

    #region Public Methods

    public void OpenClosePanel()
    {
        if (animator != null)
        {
            bool isOpen = animator.GetBool("isOpen");

            animator.SetBool("isOpen", !isOpen);
        }
    }

    #endregion

}
