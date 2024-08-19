using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

public class GDPRManager : MonoBehaviour
{
    private ConsentForm _consentForm;
    private Action _onFormShown;

    public void StartShowGDPR(Action onFormShown)
    {
        _onFormShown = onFormShown;
        var deviceId = SystemInfo.deviceUniqueIdentifier;
        
        var request = new ConsentRequestParameters()
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = new ConsentDebugSettings()
            {
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = new List<string>()
                {
#if UNITY_ANDROID
                    deviceId.ToUpper()
#elif UNITY_IOS
                    deviceId
#endif
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
            _onFormShown?.Invoke();
            _onFormShown = null;
            return;
        }

        _consentForm = consentForm;
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnConsentFormShown);
            
            _onFormShown?.Invoke();
            _onFormShown = null;
        }
    }

    private void OnConsentFormShown(FormError error)
    {
        if (error != null)
        {
            Debug.LogError(error.Message);
            _onFormShown?.Invoke();
            _onFormShown = null;
            return;
        }

        LoadConsentForm();
    }
    
    private void OnConsentInfoUpdate(FormError error)
    {
        if (error != null)
        {
            Debug.LogError(error.Message);
            _onFormShown?.Invoke();
            _onFormShown = null;
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
    }
}
