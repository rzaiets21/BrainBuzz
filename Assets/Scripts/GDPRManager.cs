using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

public class GDPRManager : MonoBehaviour
{
    private ConsentForm _consentForm;
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        var androidId = SystemInfo.deviceUniqueIdentifier;
        
        Debug.Log(androidId.ToUpper());
        
        var request = new ConsentRequestParameters()
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = new ConsentDebugSettings()
            {
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = new List<string>()
                {
                    androidId.ToUpper()
                }
            }
        };
        
        ConsentInformation.Update(request, OnConsentInfoUpdate);
    }

    private void LoadConsentForm()
    {
        ConsentForm.Load(OnConsentFormLoaded);
    }

    private void OnConsentFormLoaded(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            Debug.LogError(error.Message);
            return;
        }

        _consentForm = consentForm;
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnConsentFormShown);
        }
    }

    private void OnConsentFormShown(FormError error)
    {
        if (error != null)
        {
            Debug.LogError(error.Message);
            return;
        }

        LoadConsentForm();
    }
    
    private void OnConsentInfoUpdate(FormError error)
    {
        if (error != null)
        {
            Debug.LogError(error.Message);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
    }
}
