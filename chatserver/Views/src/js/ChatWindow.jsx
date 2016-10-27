import React from 'react';
import Message from './Message.jsx';
import InputBar from './InputBar.jsx';
import {xhttp} from 'xhttp';
import * as Charts from './charts';



export default class ChatWindow extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            messages: [],
            latestId: 0,
            timeBetweenMsg: 0,
            avgLettersPerUser: 0,
            avgLettersAllUsers: 0,
            wph: ["wph"],
            mph: ["mph"],

        };
    }

    componentDidMount() {
        this._fetchStatisticsScalars.bind(this);
        this._fetchStatisticsVectors.bind(this)
        Charts.lineChartObject = Charts.generateLineChart([["mph",0,0,0],["wph",0,0,0]], "#lineChart");
    }

    componentWillReceiveProps(newProps) {
        let fetcherMsg = window.setInterval(this._fetchLatestMessages.bind(this), 2000, newProps.username);
        let fetcherScalars = window.setInterval(this._fetchStatisticsScalars.bind(this), 60000);
        let fetcherVectors = window.setInterval(this._fetchStatisticsVectors.bind(this), 10000);
        //this.setState({fetcherId: fetcher, fetcherScalars: fetcherScalars, fetcherVectors: fetcherVectors});
    }


    render() {

        let messageList = this._buildMessageList();
        return (
            <div className="container--chat-page">
                <div className="chat--header">
                    <div className="container--statistic-scalars">
                        <div className="--"><spa>Sec/Msg</spa><span></span></div>
                    </div>
                    <div id="lineChart" className="line-chart"></div>
                </div>
                <div className="container--list-messages">
                    {messageList}
                    <div className="container--username">{this.props.username}</div>
                </div>
                <InputBar actionName="send"
                          placeholder="Type a message"
                          className="chat--input"
                          ref="messageInput"
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

        if (msgs && msgs.length > 0) {
            maxId = (msgs[msgs.length - 1]).ID;
        } else {
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
                url: "http://"+this.props.apiUrl+"/api/messages/add",
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
                url: "http://"+this.props.apiUrl+"/api/messages/get/afterid/" + this.state.latestId + "/user/" + username,
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

    _fetchStatisticsScalars(){
        xhttp({
                url: "http://"+this.props.apiUrl+"/api/messages/statistics/scalars",
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Access-Control-Allow-Origin': '*'
                },
                method: 'GET'
            },
            (data, xhr) => {

                this.setState(data);

            },
            (err, xhr) => {
                console.error(xhr.responseURL, xhr.status, xhr.statusText);
            });
    }
    _fetchStatisticsVectors(){
        xhttp({
                url: "http://"+this.props.apiUrl+"/api/messages/statistics/vectors?hours=5",
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Access-Control-Allow-Origin': '*'
                },
                method: 'GET'
            },
            (data, xhr) => {

                let wph = ["wph", ...data.wordsPerHourLastWeek]; //wordsPerHourLastWeek
                let mph = ["mph", ...data.msgPerHourLastWeek]; //msgPerHourLastWeek
                Charts.lineChartObject.load({columns: [wph, mph]});
                console.log("vectors loaded");
            },
            (err, xhr) => {
                console.error(xhr.responseURL, xhr.status, xhr.statusText);
            });
    }

}