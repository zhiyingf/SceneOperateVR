using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class SelectMenuUIController : MonoBehaviour
{
    //[Serializable]
    //public class UnityEventBool : UnityEvent<bool> { }

    public GameObject selectMenuUI;
    public GameObject HanderLeft;
    public GameObject HanderRight;

    [SerializeField]
    [FormerlySerializedAs("onIsValidChanged")]
    private UnityEvent m_onIsValidChanged;

    public UnityEvent onIsValidChanged { get { return m_onIsValidChanged; } }

    private bool showOrNot = false;

    private void Awake()
    {
        selectMenuUI.SetActive(showOrNot);
        HanderLeft.SetActive(showOrNot);
        HanderRight.SetActive(showOrNot);
    }

    public void SwitchToMenuMode()
    {
        showOrNot = !showOrNot;

        selectMenuUI.SetActive(showOrNot);
        HanderLeft.SetActive(showOrNot);
        HanderRight.SetActive(showOrNot);

        if (showOrNot && m_onIsValidChanged != null)
        {
            m_onIsValidChanged.Invoke();
        }
    }

}
