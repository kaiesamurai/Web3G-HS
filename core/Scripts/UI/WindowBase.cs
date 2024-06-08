using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WindowBase : MonoBehaviour
{
    private static List<WindowBase> instances = new();

    public WindowType type;

    private CloseWindowRequest closeRequest = null;
    public bool showOnInitialize;

    // Start is called before the first frame update
    void Awake()
    {
        instances.Add(this);
    }

    private void Start()
    {
        Initialize();
        if (showOnInitialize)
        {
            Initialized?.Invoke(this);
            Show();
        }
        else
        {
            gameObject.SetActive(false);
            Initialized?.Invoke(this);
        }
    }

    private void Update()
    {
        if (closeRequest != null)
        {
            gameObject.SetActive(false);
            if (closeRequest.Result == WindowResult.Success)
                Success?.Invoke(this, closeRequest.Data);
            else if (closeRequest.Result == WindowResult.Error)
                Error?.Invoke(this, closeRequest.Data);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    protected void Close(WindowResult result, object data)
    {
        closeRequest = new CloseWindowRequest() { Result = result, Data = data };
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    public abstract void Initialize();
    protected virtual void OnShow() {}

    public static T GetSpecificInstanceOfType<T>() where T : WindowBase
    {
        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i] is T)
                return instances[i] as T;
        }
        return null;
    }


    public event WindowCallback Success;
    public event WindowCallback Error;
    public event WindowInitializedCallback Initialized;

}
public enum WindowType { Dialog, PopUp }
public enum WindowResult { Success, Error }
public class CloseWindowRequest
{
    public WindowResult Result;
    public object Data;
}

public delegate void WindowCallback(WindowBase sender, object data);
public delegate void WindowInitializedCallback(WindowBase sender);