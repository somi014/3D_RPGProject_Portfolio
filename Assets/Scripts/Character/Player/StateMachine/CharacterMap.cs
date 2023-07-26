using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// InputSystem
/// </summary>
public partial class PlayerStateManager
{
    [Header("Character Input Values")]
    [HideInInspector] public Vector2 move;
    [HideInInspector] public Vector2 look;
    [HideInInspector] public bool jump;
    [HideInInspector] public bool sprint;
    [HideInInspector] public bool roll;
    [HideInInspector] public bool oneHandEequip;
    [HideInInspector] public bool attack;
       
    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [HideInInspector] public Vector3 mouseInputPosition;

    [HideInInspector] public bool uiOpen = false;

#if ENABLE_INPUT_SYSTEM
    public void MousePositionUpdata(InputAction.CallbackContext callbackContext)
    {
        mouseInputPosition = callbackContext.ReadValue<Vector2>();
    }

    #region Movement
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        move = callbackContext.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext callbackContext)
    {
        if (cursorInputForLook)
        {
            look = callbackContext.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (currentState == idlingState)
        {
            jump = callbackContext.action.IsPressed();
            if (Grounded == true)
            {
                SwitchState(jumpState);
            }
        }
    }

    public void OnSprint(InputAction.CallbackContext callbackContext)
    {
        sprint = callbackContext.action.IsPressed();
    }

    public void OnRoll(InputAction.CallbackContext callbackContext)
    {
        roll = callbackContext.action.IsPressed();
        if (Grounded == true)
        {
            SwitchState(rollState);
        }
    }
    #endregion

    #region Combat
    public void OnAttack(InputAction.CallbackContext callbackContext)
    {
        if (uiOpen == true)         //인벤토리 열려있으면 리턴
        {
            return;
        }

        if (currentState != idlingState)
        {
            return;
        }

        attack = callbackContext.action.IsPressed();
        if (Grounded == true)
        {
            if (currentState != attackState)
                SwitchState(attackState);
        }
    }

    public void OnHeal(InputAction.CallbackContext callbackContext)
    {
        if (currentState != idlingState)
        {
            return;
        }

        if (Grounded == true)
        {
            SwitchState(healState);
        }
    }
    #endregion

#endif

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}