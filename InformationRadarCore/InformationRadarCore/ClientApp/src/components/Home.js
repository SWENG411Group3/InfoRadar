import React, { Component } from "react";
import authService from "./api-authorization/AuthorizeService";
import apiService from "../services/ApiService";
import { Dashboard } from "./Dashboard";

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {
            loggedIn: null
        };
    }

    async componentDidMount() {
        this.setState({
            loggedIn: await authService.isAuthenticated(),
        });
    }

    render() {
        if (this.state.loggedIn) {
            return <Dashboard />
        }
        return <div>
            <h1>Welcome</h1>
            <p>Please sign in.</p>
        </div>;
    }
}
