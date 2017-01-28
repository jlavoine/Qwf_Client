﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyLibrary;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qwf {
    public class LoginScreen : MonoBehaviour {
        public const string STATUS_CONNECTING = "Connecting to server...";
        public const string STATUS_DOWNLOADING_GAME = "Connected to server -- downloading game data!";
        public const string STATUS_DOWNLOADING_PLAYER = "Connected to server -- downloading player data!";
        public const string STATUS_FAILED = "Failed to connect to server. Please close and try again.";

        public GameObject LoginFailurePopup;

        private IQwfBackend mBackend;

        private bool mBackendFailure = false;

        private Login mLogin;   // is this the best way...?

        private AnalyticsTimer mLoginTimer = new AnalyticsTimer( LibraryAnalyticEvents.LOGIN_TIME, new MyTimer() );

        public GameObject PlayButton;
        public TextMeshProUGUI LoginStatusText;

        void Start() {
            mBackend = new QwfBackend();
            UnityEngine.Debug.LogError( "Is the backend busy: " + mBackend.IsBusy() );
            //BackendManager.Init( mBackend );

            MyMessenger.AddListener( BackendMessages.LOGIN_SUCCESS, OnLoginSuccess );
            MyMessenger.AddListener<IBackendFailure>( BackendMessages.BACKEND_REQUEST_FAIL, OnBackendFailure );

            LoginStatusText.text = STATUS_CONNECTING;

            mLogin = new Login( mBackend, mLoginTimer );
            mLogin.Start();
        }

        private void DoneLoadingData() {
            if ( !mBackendFailure ) {
                LoginStatusText.gameObject.SetActive( false );
                PlayButton.SetActive( true );
                mLoginTimer.StopAndSendAnalytic();
            }
        }

        void OnDestroy() {
            mLogin.OnDestroy();
            MyMessenger.RemoveListener( BackendMessages.LOGIN_SUCCESS, OnLoginSuccess );
            MyMessenger.RemoveListener<IBackendFailure>( BackendMessages.BACKEND_REQUEST_FAIL, OnBackendFailure );
        }

        private void OnBackendFailure( IBackendFailure i_failure ) {
            if ( !mBackendFailure ) {
                mBackendFailure = true;
                //gameObject.InstantiateUI( LoginFailurePopup );    // right now this conflicts with OutOfSync popup
                LoginStatusText.text = STATUS_FAILED;
            }
        }

        private void OnLoginSuccess() {
            StartCoroutine( LoadDataFromBackend() );
        }

        private IEnumerator LoadDataFromBackend() {
            LoginStatusText.text = STATUS_DOWNLOADING_GAME;
            UnityEngine.Debug.LogError( "Is the backend busy: " + mBackend.IsBusy() );
            StringTableManager.Init( "English", mBackend );
            //Constants.Init( mBackend );
            //GenericDataLoader.Init( mBackend );
            //GenericDataLoader.LoadDataOfClass<BuildingData>( GenericDataLoader.BUILDINGS );
            //GenericDataLoader.LoadDataOfClass<UnitData>( GenericDataLoader.UNITS );
            //GenericDataLoader.LoadDataOfClass<GuildData>( GenericDataLoader.GUILDS );

            while ( mBackend.IsBusy() ) {
                yield return 0;
            }
            mLoginTimer.StepComplete( LibraryAnalyticEvents.TITLE_DATA_TIME );

            //yield return SetUpPlayerData();
            
            DoneLoadingData();
        }

        private IEnumerator SetUpPlayerData() {
            LoginStatusText.text = STATUS_DOWNLOADING_PLAYER;

            // it's possible that the client is restarting and old player data exists -- we need to dispose of it
            /*if ( PlayerManager.Data != null ) {
                PlayerManager.Data.Dispose();
            }

            PlayerData playerData = new PlayerData();
            playerData.Init( mBackend );
            PlayerManager.Init( playerData );
            */
            while ( mBackend.IsBusy() ) {
                yield return 0;
            }
            /*
            playerData.AddDataStructures();
            playerData.CreateManagers();*/
            
            mLoginTimer.StepComplete( LibraryAnalyticEvents.INIT_PLAYER_TIME );
        }
    }
}