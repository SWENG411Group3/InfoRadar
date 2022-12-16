import React, { Component } from "react";
import apiService from "../services/ApiService";
import { DashboardAdminButtons } from "./DashboardAdminButtons";
import { Lighthouses } from "./Lighthouses";

export class Dashboard extends Component {
    static displayName = Dashboard.name;

    constructor(props) {
        super(props);
        this.state = {
            isAdmin: false,
        };
    }

    componentDidMount() {
        this.checkAdmin();
    }

    async checkAdmin() {
        const isAdmin = await apiService.isAdmin();
        this.setState({ isAdmin });
    }

    render() {
        return (
            <div>
                {this.state.isAdmin &&
                    <DashboardAdminButtons />}
                <Lighthouses />
            </div>
        );
    }
}
