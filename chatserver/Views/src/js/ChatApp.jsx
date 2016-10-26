import React from 'react';
import ChatWindow from './ChatWindow.jsx';
import InputBar from './InputBar.jsx';

export default class ChatApp extends React.Component {
    constructor(props) {
        super(props)
        this.state = {username: ""};
    }

    componentWillReceiveProps(newProps) {

    }

    render() {
        let loginModal = "";

        if (this.state.username === "") {
            loginModal = (
                <div className={"fullsize centered-content blurred front-element"}>
                    <login className="centered-content modal--login">
                        <InputBar actionName="login"
                                  placeholder="Enter username"
                                  submitAction={this._login.bind(this)}/>
                    </login>
                </div>
            );
        }

        return (
            <div className="main-page">
                {loginModal}
                <ChatWindow ref=""
                            className="fullsize centered-content"
                            messages={this.mockMessages}
                            username={this.state.username}/>
            </div>)
    }

    _login(username) {
        this.setState({username: username});
    }
}

