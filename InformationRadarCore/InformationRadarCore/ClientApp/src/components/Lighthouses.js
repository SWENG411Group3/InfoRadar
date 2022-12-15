import React, { Component } from "react";
import apiService from "../services/ApiService";
import { DashboardAdminButtons } from "./DashboardAdminButtons";

export class Lighthouses extends Component {
    static displayName = Lighthouses.name;

    constructor(props) {
        super(props);
        this.state = {
            entries: [],
            isComplete: false,
            cursor: null,
            isAdmin: false,
        };
    }

    componentDidMount() {
        this.loadLighthouses();
    }

    async loadLighthouses() {
        const old = this.state.entries;
        const lighthouses = await apiService.getLighthouses();
        const isAdmin = await apiService.isAdmin();
        this.setState({
            entries: old.concat(lighthouses.entries),
            isComplete: lighthouses.isComplete,
            cursor: lighthouses.cursor,
            isAdmin,
        });
    }

    async loadTemplates() {

    }

    render() {
        return (
            <div>
                {this.state.isAdmin && 
                    <DashboardAdminButtons />}

                <table className="table table-striped" aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Enabled</th>
                            <th>Has Error</th>
                            <th>Subscribed</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.state.entries.map(lighthouse =>
                            <tr key={lighthouse.internalName}>
                                <td><a href={"/Lighthouse/" + lighthouse.id}>{lighthouse.title}</a></td>
                                <td>
                                    <input type="checkbox" 
                                        name={lighthouse.internalName + "-enabled-checkbox"}
                                        lighthouse={lighthouse.id} 
                                        readOnly
                                        checked={lighthouse.enabled} />
                                </td>
                                <td>
                                    <input type="checkbox" 
                                        name={lighthouse.internalName + "-error-checkbox"}
                                        value={lighthouse.id} 
                                        readOnly
                                        checked={lighthouse.hasError} />
                                </td>
                                <td>
                                    <input type="checkbox" 
                                        name={lighthouse.internalName + "-subscribed-checkbox"}
                                        value={lighthouse.id} 
                                        readOnly
                                        checked={lighthouse.subscribed} />
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                {!this.state.isComplete && <button className="btn btn-primary mr-1" onClick={this.loadLighthouses}>Load more</button>}
            </div>      
    );
  }
}
