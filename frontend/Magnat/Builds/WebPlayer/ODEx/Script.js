
var rParams = FAPI.Util.getRequestParameters();
FAPI.init(rParams["api_server"], rParams["apiconnection"],
    function() {
    	console.log("OD Inited!");
    },
    function(error){
    	console.log("OD Init error: "+error);
    }
);

function API_callback(method, result, data)
{
	console.log("API CALLBACK "+method);
	UnityShow();
};

function GetUnity()
{
	return u.getUnity();
}

function UnityHide()
{
	GetUnity().style.left = '8000px';
	GetUnity().style.position = 'relative';
}

function UnityShow()
{
	GetUnity().style.left = '0px';
}

function SendToPlayer(func, what)
{
	console.log("snding to player ["+func+"] : ["+what+"]");
	GetUnity().SendMessage("SocialManager",func,what);
}

function GetParams()
{
	console.log("Get params : " + document.location.href + '');
	SendToPlayer("RecvParams",document.location.href + '');
}

function OpenFileDialog()
{
	UnityHide();
	uform.css("left","0px");
}

function GetFriends(uid)
{
	FAPI.Client.call({"method":"friends.getAppUsers"}, function(status, uids, error){
		if (status == "ok")
		{
			uid ="";
			for (var i = uids["uids"].length - 1; i >= 0; i--) 
			{
				uid+=uids["uids"];
				if (i!=uids["uids"].length - 1)
					uid+=",";
			}
			FAPI.Client.call({"method":"users.getInfo", "uids":uid, "fields":"uid,first_name,last_name,pic_5", "emptyPictures":true}, function(status,data,error){
				if (status == "ok")
				{
					console.log(data);
					var res = "";
					for (var i = 0;i<data.length; i++)
					{
						res += "[";
						res += data[i]['uid']+",";
						res += data[i]['first_name']+",";
						res += data[i]['last_name']+",";
						if (data[i]['pic_5'])
							res += data[i]['pic_5'];
						else
							res += "   ";
						res += "]";
					}
					SendToPlayer("OnGetFriends",res);
				}
				else
					console.log(error);
			});
		}
		else
			console.log(error);
	});
}

function GetProfile(uid)
{
	FAPI.Client.call({"method":"users.getInfo", "uids":uid, "fields":"uid,first_name,last_name,pic_5", "emptyPictures":true}, function(status, data, error){
		console.log(data);
		var Id = "";
		var Fname = "";
		var Lname = "";
		var Photo = "       ";
		if (status == "ok")
		{
			for (var i = data.length - 1; i >= 0; i--) 
			{
				Id = data[i]['uid'];
				Fname = data[i]['first_name'];
				Lname = data[i]['last_name'];
				if (data[i]['pic_5'])
					Photo = data[i]['pic_5'];
				var Profile = [Id,Fname,Lname,Photo];
				SendToPlayer("OnGetPlayer",""+Profile);
			}
		}	
	});

}

function ShowInvite()
{
	console.log("------- showInviteBox");

	UnityHide();
	FAPI.UI.showNotification("Поиграй в мою игру!", function(res){}, null);
	/*
	VK.callMethod("showInviteBox"4);
	UnityHide();
	*/
}

function order(gid) 
{
	UnityHide();
	FAPI.UI.showPayment("Яблоко", "Это очень вкусно!", 777, 1, null, null, "ok", "true");
	/*
	var params = {
		type: 'item',
		item: gid
	};*/
	//VK.callMethod('showOrderBox', params);
}
/*
VK.addCallback('onOrderSuccess', function(order_id) {
	UnityShow();
	GetUnity().SendMessage("Bank","OnPaymentSuccessful",order_id);
});

VK.addCallback('onOrderFail', function() {
	UnityShow();
	GetUnity().SendMessage("Bank","OnPaymentError","");
});

VK.addCallback('onOrderCancel', function() {
	UnityShow();
	GetUnity().SendMessage("Bank","OnPaymentCancel","");
});
*/