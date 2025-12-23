using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace FPS.Scripts
{
    public class InputActionEntity<T> where T : struct
    {
        public InputActionEntity (InputAction inputAction)
        {
            m_inputAction = inputAction;
            inputAction.started += StartedHandler;
            inputAction.performed += PerformedHandler;
            inputAction.canceled += CanceledHandler;
        }

        public event Action<T> Started;
        public event Action<T> Performed;
        public event Action<T> Canceled;

        public void InvokeStarted(T value)
        {
            Started?.Invoke(value);
        }

        public void InvokePerformed(T value)
        {
            Performed?.Invoke(value);
        }

        public void InvokeCanceled(T value)
        {
            Canceled?.Invoke(value);
        }

        public void Dispose()
        {
            m_inputAction.started -= StartedHandler;
            m_inputAction.performed -= PerformedHandler;
            m_inputAction.canceled -= CanceledHandler;
        }
   
        private InputAction m_inputAction;
   
        private void StartedHandler(InputAction.CallbackContext ctx)
        {
            T value = ctx.ReadValue<T>();
            InvokeStarted(value);
        }

        private void PerformedHandler(InputAction.CallbackContext ctx)
        {
            T value = ctx.ReadValue<T>();
            InvokePerformed(value);
        }

        private void CanceledHandler(InputAction.CallbackContext ctx)
        {
            T value = ctx.ReadValue<T>();
            InvokeCanceled(value);
        }
    }
}

