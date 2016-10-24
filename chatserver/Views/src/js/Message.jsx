import React from 'react';

export default class Message extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className={"message " + ((this.props.mine) ? "mine-message" : "guest-message")} >
                <span className="message-text">{this.props.text}</span>
            </div>
        );
    }
}