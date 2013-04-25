boxcar-api
==========
boxcar push notification service .net api

Usage
-----------------
First, you'll need to create a provider [here](http://boxcar.io/site/providers). 
And get a provider key and a provider secret. 

    BoxcarApi api = new BoxcarApi("your_api_key", "your_api_secret");
    
Subscribe a Boxcar user to your service:
    
    var result = api.Subscribe("user@example.com");

Then send that user a notification:

	var result = api.Notify("user@example.com", "This is a test message!..");
        
Or reach all of your subscribers with a broadcast:

    var result = api.Broadcast("This message is broadcasted to all users.");