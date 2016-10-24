import React from 'react';
import Message from './Message.jsx';
import InputBar from './InputBar.jsx';
import {xhttp} from 'xhttp';


export default class ChatWindow extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            messages: [],
            latestId: 0
        };
    }


    componentWillReceiveProps(newProps) {
        let fetcher = window.setInterval(this._fetchLatestMessages.bind(this), 2000, newProps.username);
        //this._fetchLatestMessages(newProps.username);
        this.setState({fetcherId: fetcher});
    }


    render() {
        let messageList = this._buildMessageList();
        return (
            <div className="container--chat-page">
                <div className="chat--header">{this.props.username}</div>
                <div className="container--list-messages">
                    {messageList}
                </div>
                <InputBar actionName="send"
                          placeholder="Type a message"
                          className="chat--input"
                          ref = "messageInput"
                          value=""
                          submitAction={this._addMessage.bind(this)}/>
            </div>
        );
    }


    /***
     * Adding a list of newely received messages to our current messages
     * @param msgs
     * @private
     */
    _addMessages(msgs) {
        let maxId;

        if(msgs && msgs.length > 0){
            maxId = (msgs[msgs.length - 1]).ID;
        }else{
            return;
        }


        this.setState({
            messages: [...this.state.messages, ...msgs],
            latestId: maxId
        });
    }

    /***
     * Send one message from the current user and reload the conversation
     * @param text
     * @private
     */
    _addMessage(text = "") {
        let slimMessage = {};
        slimMessage.TEXT = text;
        slimMessage.FROM = this.props.username;
        slimMessage.TO = "";

        xhttp({
                url: "http://localhost:34778/api/messages/add",
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Access-Control-Allow-Origin': '*'
                },
                body: slimMessage,
                method: 'PUT'
            },
            (data, xhr) => {
                this.refs.messageInput.value = "";
                console.log("message: " + text + " was put.");
                this._fetchLatestMessages(this.props.username); //update the message list
            },
            (err, xhr) => {
                console.error(xhr.responseURL, xhr.status, xhr.statusText);
            });
    }

    /***
     * Fetch messages from the server for a specified user
     * @private
     */
    _fetchLatestMessages(username) {
        xhttp({
                url: "http://localhost:34778/api/messages/get/afterid/" + this.state.latestId + "/user/" + username,
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Access-Control-Allow-Origin': '*'
                },
                method: 'GET'
            },
            (data, xhr) => {
                this._addMessages(data);
            },
            (err, xhr) => {
                console.error(xhr.responseURL, xhr.status, xhr.statusText);
            });
    }

    /***
     * Massage the raw list into react elements list
     * @param messages
     * @returns {Array}
     * @private
     */
    _buildMessageList(messages) {
        var messageList = [];
        let self = this;
        this.state.messages.map(function (message, inx, origArr) {
            messageList.push(<Message text={message.TEXT}
                                      from={message.FROM}
                                      to={message.TO}
                                      mine={(self.props.username === message.FROM)}
                                      key={inx}/>)
        });
        return messageList;
    }


}