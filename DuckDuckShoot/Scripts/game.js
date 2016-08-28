var name = "";
var players = [];
var users = [];
var game;
var roundTimer = 0;
var roundTimerInterval;
var loaded = false;
var outcomeToString = function(outcome) {
    var command = outcome["ActCommand"];
    if (command["ActType"] === 0) {
        var outcomeStr = command["Actor"]["PlayerUser"]["Name"] +
            " shot " +
            command["Target"]["PlayerUser"]["Name"];
        if (outcome["TargetDucked"]) {
            return outcomeStr + " but they ducked.";
        } else {
            return outcomeStr + ".";
        }
    } else {
        return command["Actor"]["PlayerUser"]["Name"] + " ducked (but wasn't shot at).";
    }
};
var populateGame = function() {
    var isAlive = true;
    var player;
    for (var i = 0; i < players.length; i++) {
        player = players[i];
        if (player["PlayerUser"]["Name"] === name && !player["IsAlive"]) {
            isAlive = false;
        }
    }
    for (var j = 0; j < players.length; j++) {
        player = players[j];
        if (player["IsAlive"]) {
            if (isAlive) {
                addPlayer(player);
            }
        }
    }
}
$(function() {
    // Declare a proxy to reference the hub.
    game = SJ.iwc.SignalR.getHubProxy('game',
    {
        client: {
            addUser: function(user) {
                if (!loaded) {
                    if (user["Name"] === name) {
                        loaded = true;
                    }
                    return;
                }
                var userName = user["Name"];
                var encodedName = $('<div />').text(userName).html();
                $('#lobby').append("<span class='lobbyUser' id='user-" + userName + "'>" + encodedName + '</span>');
                console.log(encodedName);
                users.push(user);
            },
            removeUser: function(user) {
                if (!loaded) {
                    return;
                }
                var userName = user["Name"];
                $("#user-" + userName).remove();
                var index = -1;
                for (var i = 0; i < users.length; i++) {
                    if (users[i]["Name"] === userName) {
                        index = i;
                    }
                }
                if (index !== -1) {
                    users.splice(index, 1);
                }
            },
            gameStart: function(playerList) {
                if (!loaded) {
                    return;
                }
                console.log("Starting new game...");
                $("#game").show();
                $("#gamesetup").hide();
                $("#outcomes").hide();
                players = playerList;
                populateGame();
            },
            turnStart: function (state) {
                console.log("Starting new turn...");
                if (!loaded) {
                    return;
                }
                players = state["players"];
                roundTimer = 60;
                $("#timerValue").text(roundTimer);
                roundTimerInterval = window.setInterval(function() {
                        roundTimer--;
                        if (roundTimer <= 0) {
                            window.clearInterval(roundTimerInterval);
                        }
                        $("#timerValue").text(roundTimer);
                    },
                    1000);
                deleteAllUsers();
                populateGame();
            },
            getOutcomes: function(outcomes) {
                if (!loaded) {
                    return;
                }
                $("#outcomes").empty();
                $("#outcomes").show();
                for (var i = 0; i < outcomes.length; i++) {
                    var outcome = outcomes[i];
                    $('#outcomes').append("<span>- " + outcomeToString(outcome) + "</span>");
                }
                game.server.sendReady();
            },
            gameEnd: function(winners) {
                if (!loaded) {
                    return;
                }
                var winnerString = "";
                var sep = "";
                for (var i = 0; i < winners.length; ++i) {
                    winnerString += sep + winners[i]["PlayerUser"]["Name"];
                    sep = ", and ";
                }
                $('#outcomes').append("<span>- Winner/s: " + winnerString + "</span>");
                $('#suddenDeathBtn').hide();
                $("#gamesetup").show();
            },
            suddenDeathStart: function () {
                if (!loaded) {
                    return;
                }
                $("#suddenDeathBtn").show();
            },
            receiveChatMessage: function (user, message) {
                if (!loaded) {
                    return;
                }
                $("#chatLog").append("<span><b>" + user["Name"] + "</b>: " + message + " </span>");
                $('#chat').scrollTop($('#chat')[0].scrollHeight);
            }
        }

    });
    // Set initial focus to message input box.
    $('#message').focus();

    var setupPage = function() {
        game.server.getCurrentLobbyState()
            .done(function(state) {
                users = state["users"];
                players = state["players"];
                var gameInProgress = state["gameInProgress"];
                for (var i = 0; i < users.length; i++) {
                    var userName = users[i]["Name"];
                    var encodedName = $('<div />').text(userName).html();
                    $('#lobby').append("<span id='user-" + userName + "'>" + encodedName + '</span>');
                }
                if (gameInProgress) {
                    game.client.gameStart(state);
                }
            });

        // Bind start game button handler
        $("#startGame")
            .click(function() {
                game.server.startGame();
            });

        // Bind chat button handler
        $("#chatSend")
            .click(function() {
                game.server.broadcastChatMessage($("#chatText").val());
                $("#chatText").val("");
                console.log("V: "+$("#chatText").val());
            });
        // Bind hitting enter in the chat box
        $("#chatText")
            .keyup(function (e) {
                if (e.keyCode == 13) {
                    $("#chatSend").click();
                }
            });
        // Bind sudden death button handler
        $("#suddenDeathBtn")
            .click(function () {
                game.server.suddenDeathShoot();
            });
    }
    // Start the connection.
    SJ.iwc.SignalR.start()
        .done(function() {
            console.log("Connection to Server Successful");
            var handleNameReturn = function(success) {
                if (success) {
                    setupPage();
                } else {
                    name = prompt("That name is taken. Please enter a different one.");
                    game.server.newName(name).done(handleNameReturn);
                }
            };
            game.server.getName()
                .done(function(servername) {
                    if (servername == undefined) {
                        name = prompt("Enter your name");
                        game.server.newName(name).done(handleNameReturn);
                    } else {
                        name = servername;
                        loaded = true;
                        setupPage();
                    }
                });
        });
});

//Distribute the players around the circle
function distributePlayers(deg) {
    deg = deg || 0;
    var count = 1;
    //Radius of circle
    var radius = 200;
    var fields = $('.field:not([deleting])'),
        container = $('#container'),
        width = container.width(),
        height = container.height(),
        angle = deg || Math.PI * 4.5,
        step = (2 * Math.PI) / fields.length;

    //Go through each player and set up position + call sprite
    fields.each(function () {
        var x = Math.round(width / 2 + radius * Math.cos(angle) - $(this).width() / 2);
        var y = Math.round(height / 2 + radius * Math.sin(angle) - $(this).height() / 2);

        $(this).css({
            left: x + 'px',
            top: y + 'px',
        });

        angle += step;

        //400, 300 is the center point, so get the angle from there
        setSprite('stand', $(this).find('img').attr("id"), 400, 300, x + 55, y + 55);
    });
}

var incrementUsers = 0;

//Add a new player with a username
function addPlayer(player) {
    var username = player["PlayerUser"]["Name"];
    if (username !== name) {
        $('<div/>',
            {
                'class': 'field'
            })
            .append(
                $('<p/>', { 'class': 'username', 'text': username }),
                $('<span/>', {'text': player["NumDucks"] + " Ducks Left"}),
                $('<img/>', { 'src': imagePath + 's.png', 'id': username, 'class': 'player' }),
                $('<button/>',
                {
                    'class': 'shootButton',
                    'id': 'shoot_' + username,
                    'text': 'Shoot'
                })
            )
            .css({
                left: $('#container').width() / 2 - 25 + 'px',
                top: $('#container').height() / 2 - 25 + 'px'
            })
            .addClass('anim')
            .appendTo('#container')
        distributePlayers();
    } else {
        $('<div/>',
            {
                'class': 'field'
            })
            .append(
                $('<p/>', { 'class': 'username', 'text': username }),
                $('<span/>', { 'text': player["NumDucks"] + " Ducks Left" }),
                $('<img/>', { 'src': imagePath + 's.png', 'id': username, 'class': 'player' }),
                $('<button/>',
                {
                    'class': 'duckButton',
                    'id': 'duck_' + username,
                    'text': 'Duck'
                })
            )
            .css({
                left: $('#container').width() / 2 - 25 + 'px',
                top: $('#container').height() / 2 - 25 + 'px'
            })
            .addClass('anim')
            .appendTo('#container');
        if (player["NumDucks"] == 0) {
            $(".duckButton").hide();
        }
        distributePlayers();
    }

}
function deleteAllUsers() {
  

    $('.field').remove();

    distributePlayers();
}

function deleteUser(username) {
    $('#' + username)
    .attr('deleting', 'true')
    .css({
        left: $('#container').width() / 2 - 25 + 'px',
        top: $('#container').height() / 2 - 25 + 'px'
    })
    .fadeOut(400, function () {
        $(this).parent().remove();
    });

    distributePlayers();
}

distributePlayers();

function setSprite(action, id, currentX, currentY, targetX, targetY) {
    var mx = targetX - currentX;
    var my = targetY - currentX;
    var angle = Math.atan2(my, mx) * 180 / Math.PI;
    angle = Math.round(angle);
    if (angle < 0) {
        angle = angle + 360;
    }
    var spriteNum = Math.floor(angle / 22);


    var shootimages = [
        'es.png',
        'ses.png',
        'ses.png',
        'ss.png',
        'ss.png',
        'sws.png',
        'sws.png',
        'ws.png',
        'ws.png',
        'nws.png',
        'nws.png',
        'ns.png',
        'ns.png',
        'nes.png',
        'nes.png',
        'es.png'
    ];

    var standimages = [
        'w.png',
        'nw.png',
        'nw.png',
        'n.png',
        'n.png',
        'ne.png',
        'ne.png',
        'e.png',
        'e.png',
        'se.png',
        'se.png',
        's.png',
        's.png',
        'sw.png',
        'sw.png',
        'w.png'
    ];

    var deadimages = [
        'deadimg.png'
    ];

    var duckimages = [
       'duck.png'
    ];
    console.log(id);
    if (action == 'stand') {
        document.getElementById(id).src = imagePath + standimages[spriteNum];
    }

    if (action == 'shoot') {
        console.log(id);
        document.getElementById(id).src = imagePath + shootimages[spriteNum];
    }

    if (action == 'dead') {
        document.getElementById(id).src = imagePath + deadimages[0];

    }
    if (action == 'duck') {
        document.getElementById(id).src = imagePath + duckimages[0];

    }
}

$(document).on('click', '.shootButton', function () {
    var grabID = this.id.substr(6);
    console.log(grabID);
    var grabX = parseInt($(this).parent().css("left"));
    var grabY = parseInt($(this).parent().css("top"));
    //The ID in this case should be the person shooting, change when the
    //current person's ID can be accessed
    setSprite('shoot', name, 345, 45, grabX + 55, grabY + 55);
    //setSprite('dead', grabID, 345, 45, grabX + 55, grabY + 55)
    game.server.sendAction("SHOOT " + grabID);
  
});

$(document).on('click', '.duckButton', function () {
    var grabID = this.id.substr(5);
    console.log(grabID);
    var grabX = parseInt($(this).parent().css("left"));
    var grabY = parseInt($(this).parent().css("top"));
    //The ID in this case should be the person shooting, change when the
    //current person's ID can be accessed
    setSprite('duck', name, 345, 45, grabX, grabY);
    //setSprite('dead', grabID, 345, 45, grabX + 55, grabY + 55)
    game.server.sendAction("DUCK");

});
                  
