//Sample provided by Fabio Galuppo 
//June 2015 

function WebSocketReactiveStream(webSocketUrl, numSubjects, onjsonmessageFunc, onopenFunc, oncloseFunc) {
	this.webSocketUrl = webSocketUrl;
	this.ws = null;	
	this.subjects = [];
	this.dispose = function () {
		if (this.ws) {
            this.ws.close();
        }
	};

	var support = 'WebSocket' in window ? 'WebSocket' : null;
	if (support == null) {
    	appendMessage('* ' + noSupportMessage + '<br/>');
        return;
    }

    appendMessage('* Connecting to server ..<br/>');
    this.ws = new window[support](this.webSocketUrl);

    for (var i = 0; i < numSubjects; ++i) {
    	this.subjects[i] = new Rx.Subject();
	}

	this.ws.onopen = onopenFunc;
	this.ws.onclose = oncloseFunc;
	this.ws.onmessage = function (evt) { 
		var msg = JSON.parse(evt.data);
		if ('Tag' in msg) {
			onjsonmessageFunc(msg);
		}
		else {
			throw 'No Tag property in message';
		}
	};
}
