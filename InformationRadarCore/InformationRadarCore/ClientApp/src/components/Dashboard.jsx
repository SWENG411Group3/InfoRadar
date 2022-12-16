import React, { Component } from "react";
import apiService from "../services/ApiService";
import { DashboardAdminButtons } from "./DashboardAdminButtons";
import { Lighthouses } from "./Lighthouses";
import { Templates } from "./Templates";

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
        this.setState({
            isAdmin,
            lighthouseHandler: this.state.lighthouseHandler,
            templateHandler: this.state.templateHandler, 
        });
    }

    onLighthouseHandler(handler) {
        this.setState({
            isAdmin: this.state.isAdmin,
            lighthouseHandler: handler,
            templateHandler: this.state.templateHandler, 
        });
    }

    onTemplateHandler(handler) {
        this.setState({
            isAdmin: this.state.isAdmin,
            lighthouseHandler: this.state.lighthouseHandler,
            templateHandler: handler,
        });
    }

    onLighthouse(lighthouse) {
        if (this.state.lighthouseHandler) {
            this.state.lighthouseHandler(lighthouse);
        }
    }

    onTemplate(template) {
        if (this.state.templateHandler) {
            this.state.templateHandler(template);
        }
    }

    render() {
        return (
            <div>
                <h2>Dashboard</h2>
                {this.state.isAdmin &&
                    <DashboardAdminButtons
                        onNewLighthouse={this.onLighthouse.bind(this)} />}
                <Lighthouses getHandler={this.onLighthouseHandler.bind(this)} />
                <Templates />
            </div>
        );
    }
}
