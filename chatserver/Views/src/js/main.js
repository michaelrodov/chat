import React from 'react';
import ReactDOM from 'react-dom';
import ChatApp from './ChatApp.jsx';
import $ from 'jquery';
import 'ms-signalr-client';

var server = "localhost:34778";

var connection = $.hubConnection('http://'+server);



ReactDOM.render(<ChatApp apiUrl="localhost:34778" connection={connection}/>, document.getElementById('react-container'));