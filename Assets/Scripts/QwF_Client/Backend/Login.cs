using MyLibrary;
using UnityEngine;
using System.Collections.Generic;

namespace Qwf {
    public class Login {

        private string mLoginID;
        private IBasicBackend mBackend;  
        private IAnalyticsTimer mLoginTimer;

        public Login( IBasicBackend i_backend, IAnalyticsTimer i_loginTimer, string i_loginID ) {
            mBackend = i_backend;
            mLoginID = i_loginID;

            mLoginTimer = i_loginTimer;
            mLoginTimer.Start();
        }

        public void Start() {
            MyMessenger.AddListener<IAuthenticationSuccess>( BackendMessages.AUTH_SUCCESS, OnAuthenticationSucess );            
            MyMessenger.AddListener<IBackendFailure>( BackendMessages.BACKEND_REQUEST_FAIL, OnBackendFailure );

            mLoginTimer.Start();

            mBackend.Authenticate( mLoginID );
            //mBackend.Authenticate( SystemInfo.deviceUniqueIdentifier );
            //mBackend.Authenticate( TestUsers.FOUR );
        }

        public void OnDestroy() {
            MyMessenger.RemoveListener<IAuthenticationSuccess>( BackendMessages.AUTH_SUCCESS, OnAuthenticationSucess );
            MyMessenger.RemoveListener<IBackendFailure>( BackendMessages.BACKEND_REQUEST_FAIL, OnBackendFailure );
        }

        private void OnAuthenticationSucess( IAuthenticationSuccess i_success ) {
            mLoginTimer.StepComplete( LibraryAnalyticEvents.AUTH_TIME );

            MyMessenger.Send<LogTypes, string, string>( MyLogger.LOG_EVENT, LogTypes.Info, "Authenticate success", "" );

            mBackend.MakeCloudCall( "onLogin", null, OnLogin );
        }

        private void OnLogin( Dictionary<string, string> i_result ) {
            mLoginTimer.StepComplete( LibraryAnalyticEvents.ON_LOGIN_TIME );
            QwfBackend backend = (QwfBackend) mBackend;
            backend.SetLoggedInTime();

            MyMessenger.Send<LogTypes, string, string>( MyLogger.LOG_EVENT, LogTypes.Info, "Login success", "" );

            MyMessenger.Send( BackendMessages.LOGIN_SUCCESS );
        }

        private void OnBackendFailure( IBackendFailure i_failure ) {
            MyMessenger.Send<LogTypes, string, string>( MyLogger.LOG_EVENT, LogTypes.Info, i_failure.GetMessage(), "" );
            mLoginTimer.StopTimer();
        }
    }
}