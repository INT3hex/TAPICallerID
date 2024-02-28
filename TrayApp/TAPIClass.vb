Imports System.Net
Imports TAPI3Lib
Namespace namespace_tapi
    Public Class TAPIClass
        Private WithEvents gobjTapi As TAPI3Lib.TAPI
        Private gobjAddress As ITAddress
        Private glngToken As Long
        Private Const MediaAudio As Integer = 8
        Private Const TAPI3_ALL_TAPI_EVENTS = TAPI_EVENT.TE_CALLNOTIFICATION

        'Read from Configuration
        Public sSIPAddr As String = App._appConfig.GetProperty("sSIPAddr", "SIPTAPI 001: sip:TAPIuser@192.168.178.1")

        Public Function Initialize() As String
            Dim TAPI As New TAPI
            Dim MediaTypes As Integer
            TAPI.Initialize()
            gobjTapi = TAPI
            TAPI = Nothing
            Dim AddressCollection As ITCollection = gobjTapi.Addresses()
            For Each Address As ITAddress In AddressCollection
                If Address.State = ADDRESS_STATE.AS_INSERVICE Then
                    ' next line just qualifies a specific TAPI provider
                    If Address.AddressName = sSIPAddr Then
                        Dim MediaSupport As ITMediaSupport = Address
                        MediaTypes = MediaSupport.MediaTypes
                        MediaSupport = Nothing
                        If MediaTypes And MediaAudio = MediaAudio Then
                            gobjAddress = Address
                            Exit For
                        End If
                    End If
                End If
            Next
            If Not gobjAddress Is Nothing Then
                gobjTapi.EventFilter = TAPI3_ALL_TAPI_EVENTS
                glngToken = gobjTapi.RegisterCallNotifications(gobjAddress, True, False, MediaAudio, 1)
                Initialize = gobjAddress.AddressName
            Else
                Initialize = "Initializing failed"
            End If
        End Function
        Private Sub gobjTapi_Event(ByVal TapiEvent As TAPI3Lib.TAPI_EVENT, ByVal pEvent As Object) Handles gobjTapi.Event
            DebugPrint("TAPIEvent: " & TapiEvent)
            Select Case TapiEvent
                Case TAPI_EVENT.TE_CALLNOTIFICATION 'Call Notification Arrived
                    Dim CallNotificationEvent As ITCallNotificationEvent
                    CallNotificationEvent = CType(pEvent, ITCallNotificationEvent)

                    Debug.Print(CallNotificationEvent.Call.CallInfoString(CALLINFO_STRING.CIS_CALLERIDNUMBER))
                    RaiseEvent IncommingCall(CallNotificationEvent.Call.CallInfoString(CALLINFO_STRING.CIS_CALLERIDNUMBER), CallNotificationEvent.Call.CallInfoString(CALLINFO_STRING.CIS_CALLERIDNAME))
            End Select
        End Sub
        Public Sub ShutDown()
            gobjTapi.UnregisterNotifications(glngToken)
            gobjAddress = Nothing
            glngToken = Nothing
            gobjTapi.Shutdown()
        End Sub
        Public Event IncommingCall(ByVal strCallerID, ByVal strCallerIDName)
    End Class
End Namespace
