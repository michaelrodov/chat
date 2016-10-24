import React from 'react';

/***
 * Input window and submit button
 * Actions for submit are received by prop
 */
export default class InputBar extends React.Component {
    constructor(props) {
        super(props);
        this.state = {username: ""};
    }

    componentWillReceiveProps(newProps){
        if(this.props.inputValue !== newProps.value) {
            this.refs.input.value =  newProps.value;
        }
    }

    _updateInput(event) {
        this.setState({username: event.target.value});
    }

    //onClick={this.props.submitAction(this.state.username)}
    render() {
        var submit = this.props.submitAction;
        return (
            <div className="container--input-bar">
                <div className="inner-flex">
                    <input placeholder={this.props.placeholder}
                           type="text"
                           ref="input"
                           onKeyDown={(typeof submit == "function") ? (event)=> {
                               if(event.keyCode==13){
                                   submit(this.state.username);
                               }
                           } : () => {}}
                            onChange={this._updateInput.bind(this)}/>
                    <div className="button-submit"
                         onClick={(typeof submit == "function") ? (event)=> {submit(this.state.username)} : () => {}}>
                        {this.props.actionName}
                    </div>
                </div>
            </div>
        );
    }
}