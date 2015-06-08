

//#define SA_DEBUG_MODE
using UnityEngine;
using UnionAssets.FLE;
using System.Collections;

public class RTM_Game_Example : AndroidNativeExampleBase {
	
	
	
	public DefaultPreviewButton connectButton;
	
	
	public DefaultPreviewButton showRoomButton;
	public string test = "test";
	
	
	
	private Texture defaulttexture;

	void OnGUI(){
		GUI.Label (new Rect (10, 90, 200, 20), test);
	
	}
	
	void Start() {
		
		//		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;
		
		//listen for GooglePlayConnection events
		GooglePlayInvitationManager.ActionInvitationReceived += OnInvite;
		GooglePlayInvitationManager.ActionInvitationAccepted += ActionInvitationAccepted;
		GooglePlayRTM.ActionRoomCreated += OnRoomCreated;
		
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
		GooglePlayConnection.instance.addEventListener(GooglePlayConnection.CONNECTION_RESULT_RECEIVED, OnConnectionResult);
		
		
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 
		
		//networking event
		
		
		
	}
	
	private void ConncetButtonPress() {
		test = "Connect";
		Debug.Log("GooglePlayManager State  -> " + GooglePlayConnection.state.ToString());
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
	
			GooglePlayConnection.instance.disconnect ();
		} else {
			GooglePlayConnection.instance.connect ();
		}
	}
	
	
	private void ShowWatingRoom() {
		GooglePlayRTM.instance.ShowWaitingRoomIntent();
	}
	
	
	/*
	private static int ROLE_FARMER = 0x1; // 001 in binary
	private static int ROLE_ARCHER = 0x2; // 010 in binary
	private static int ROLE_WIZARD = 0x4; // 100 in binary


	private static int TRACK_1 = 1; // 100 in binary
	private static int TRACK_2 = 2; // 100 in binary
*/
	
	private void findMatch() {
		/*
		GooglePlayRTM.instance.SetExclusiveBitMask (ROLE_WIZARD);
		GooglePlayRTM.instance.SetVariant (TRACK_1);
*/test = "Find Match";
		int minPlayers = 1;
		int maxPlayers = 2;
		
		GooglePlayRTM.instance.FindMatch(minPlayers, maxPlayers);
	}
	
	private void InviteFriends() {
		int minPlayers = 1;
		int maxPlayers = 2;
		GooglePlayRTM.instance.OpenInvitationBoxUI(minPlayers, maxPlayers);
	}
	
	private void OnConnectionResult(CEvent e) {
		
		GooglePlayConnectionResult result = e.data as GooglePlayConnectionResult;
		Debug.Log(result.code.ToString());
	}
	
	private void OnPlayerDisconnected() {
	}
	
	private void OnPlayerConnected() {	
		
		GooglePlayInvitationManager.instance.RegisterInvitationListener();
		
		
		GooglePlayManager.instance.LoadFriends();
	}
	
	private void SendHello() {
		#if (UNITY_ANDROID && !UNITY_EDITOR) || SA_DEBUG_MODE
		string msg = new Vector3 (0,0,0).ToString();
		System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
		byte[] data = encoding.GetBytes(msg);
		
		GooglePlayRTM.instance.SendDataToAll(data, GP_RTM_PackageType.RELIABLE);
		#endif
		
	}
	
	
	
	
	
	
	void FixedUpdate() {
		
		
		if (GooglePlayRTM.instance.currentRoom.status != GP_RTM_RoomStatus.ROOM_VARIANT_DEFAULT && GooglePlayRTM.instance.currentRoom.status != GP_RTM_RoomStatus.ROOM_STATUS_ACTIVE) {
			showRoomButton.EnabledButton ();
		} else {
			showRoomButton.DisabledButton ();
		}
		
		
		
		if (GooglePlayRTM.instance.currentRoom.status == GP_RTM_RoomStatus.ROOM_STATUS_ACTIVE) {
			test = "Load GameScene";
			Application.LoadLevel ("GameScene");
			
		} 
		
	}
	
	private string inviteId;
	
	private void OnInvite(GP_Invite invitation) {
		
		if (invitation.InvitationType != GP_InvitationType.INVITATION_TYPE_REAL_TIME) {
			return;
		}
		
		inviteId = invitation.Id;
		
		AndroidDialog dialog =  AndroidDialog.Create("Invite", "You have new invite from: " + invitation.Participant.DisplayName, "Manage Manually", "Open Google Inbox");
		dialog.OnComplete += OnInvDialogComplete;
	}
	
	void ActionInvitationAccepted (GP_Invite invitation) {
		
		Debug.Log("ActionInvitationAccepted called");
		
		if (invitation.InvitationType != GP_InvitationType.INVITATION_TYPE_REAL_TIME) {
			return;
		}
		
		
		Debug.Log("Starting The Game");
		//make sure you have prepared your scene to start the game before you accepting the invite. Room join even will be triggered
		GooglePlayRTM.instance.AcceptInvitation(invitation.Id);
	}
	
	private void OnRoomCreated(GP_GamesStatusCodes code) {

	}
	
	
	
	private void OnInvDialogComplete(AndroidDialogResult result) {
		
		
		
		//parsing result
		switch(result) {
		case AndroidDialogResult.YES:
			AndroidDialog dialog =  AndroidDialog.Create("Manage Invite", "Would you like to accept this invite?", "Accept", "Decline");
			dialog.OnComplete += OnInvManageDialogComplete;
			break;
		case AndroidDialogResult.NO:
			GooglePlayRTM.instance.OpenInvitationInBoxUI();
			break;
			
		}
	}
	
	private void OnInvManageDialogComplete(AndroidDialogResult result) {
		switch(result) {
		case AndroidDialogResult.YES:
			GooglePlayRTM.instance.AcceptInvitation(inviteId);
			break;
		case AndroidDialogResult.NO:
			GooglePlayRTM.instance.DeclineInvitation(inviteId);
			break;
		}
	}
	
	
	
	
	
	
	
}	
