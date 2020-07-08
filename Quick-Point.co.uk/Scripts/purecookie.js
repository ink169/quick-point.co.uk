// --- Config --- //
var purecookieTitle = "Terms of Use"; // Title
var purecookieDesc = "By using this website, you automatically accept that we use cookies."; // Description
var purecookieLink = '<a href="cookie-policy.html" target="_blank">What for?</a>'; // Cookiepolicy link
var privacyLink = '<a href="cookie-policy.html" target="_blank">What for?</a>'; // Cookiepolicy link
var purecookieButton = "Understood"; // Button text
// ---        --- //


function pureFadeIn(elem, display) {
    var el = document.getElementById(elem);
    el.style.opacity = 0;
    el.style.display = display || "block";

    (function fade() {
        var val = parseFloat(el.style.opacity);
        if (!((val += .02) > 1)) {
            el.style.opacity = val;
            requestAnimationFrame(fade);
        }
    })();
};
function pureFadeOut(elem) {
    var el = document.getElementById(elem);
    el.style.opacity = 1;

    (function fade() {
        if ((el.style.opacity -= .02) < 0) {
            el.style.display = "none";
        } else {
            requestAnimationFrame(fade);
        }
    })();
};

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

function cookieConsent() {
    document.body.innerHTML += '<div class="cookieConsentContainer" id="cookieConsentContainer"><div class="cookieTitle"><a>' + purecookieTitle + '</a></div><div class="cookieDesc"><p>' + purecookieDesc + ' '
        + purecookieLink + '&nbsp;' + privacyLink + '</p></div><div class="cookieButton"><a onClick="purecookieDismiss();">'
        + purecookieButton + '</a></div></div>';
    pureFadeIn("cookieConsentContainer");
}

function cookieConsent_OLD() {
    if (!getCookie('purecookieDismiss')) {
        document.body.innerHTML += '<div class="cookieConsentContainer" id="cookieConsentContainer"><div class="cookieTitle"><a>' + purecookieTitle + '</a></div><div class="cookieDesc"><p>' + purecookieDesc + ' ' + purecookieLink + '</p></div><div class="cookieButton"><a onClick="purecookieDismiss();">' + purecookieButton + '</a></div></div>';
        pureFadeIn("cookieConsentContainer");
    }
}

function purecookieDismiss() {
    // setCookie('purecookieDismiss','1',7);
    pureFadeOut("cookieConsentContainer");
    startChat();

}



// https://github.com/microsoft/BotFramework-WebChat/issues/1397
//  directLine: window.WebChat.createDirectLine({ secret: 'EVhLppJqzCk.zsB5oaTgXsCbe7qH77_ZBhreJZoYsLIW8J93p5BkWvI' }),


function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
}


function startChat() {

    var d1 = window.WebChat.createDirectLine({ secret: '-OSA01tU5Z4.1ZBEedP4auYcxnJok0Vpj8BUeS-jR1-lPpmFiUeGdD0' });


    //var userId = guid().toUpperCase();
    //var userName = 'User-' + Math.floor((1 + Math.random()) * 10000);
    var userId = "24ae205b-2b29-489d-95ef-1b3d32886a0d";
    var userName = "botInitializr";

    const styleOptions = {
        rootwidth: 'Auto',
        backgroundColor: 'AliceBlue',
        botAvatarImage: 'https://github.com/ink169.png?size=64',
        bubbleBackground: 'rgba(46, 115, 187, .2)',
        //botAvatarInitials: 'BF',
        userAvatarImage: 'https://github.com/FreddieK01.png?size=64',
        bubbleFromUserBackground: 'rgba(141, 252, 237, .7)',
        bubbleMaxWidth: 600,
        hideUploadButton: true,
        // userAvatarInitials: 'WC'
    };

    window.WebChat.renderWebChat({ directLine: d1, styleOptions }, document.getElementById('webchat'));

    var activity = {
        from: {
            id: userId,
            name: userName
        },
        name: 'startConversation',
        type: 'event',
        value: '',
        channelData: {
            "personId": userId,
            "environment": window.location.host
        }
    };

    d1.postActivity(activity).subscribe(function (id) {
        //alert('trigger requestWelcomeDialog');
        //if (console) {
        //    console.log('"trigger requestWelcomeDialog" sent');
        //}
    });
}



window.onload = function () {
    cookieConsent();
}; 