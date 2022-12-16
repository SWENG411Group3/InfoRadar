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
            cursor: "",
        };
        if (props.getHandler) {
            props.getHandler(this.handler.bind(this));
        }
    }

    componentDidMount() {
        this.loadLighthouses();
    }

    async loadLighthouses() {
        const old = this.state.entries;
        const lighthouses = await apiService.getLighthouses({
            cursor: this.state.cursor,
        });
        this.setState({
            entries: old.concat(lighthouses.entries),
            isComplete: lighthouses.isComplete,
            cursor: lighthouses.cursor,
        });
    }

    handler(lighthouse) {
        this.setState({
            entries: [lighthouse, ...this.state.entries],
            isComplete: this.state.isComplete,
            cursor: this.state.cursor,
        });
    }

    body() {
        if (this.state.entries.length > 0) {
            return <div className="container">
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
                                    {lighthouse.enabled ? <span className="text-success">True</span> : <span className="text-muted">False</span>}
                                </td>
                                <td>
                                    {lighthouse.hasError ? <span className="text-danger">True</span> : <span className="text-success">False</span>}
                                </td>
                                <td>
                                    {lighthouse.subscribed ? <span className="text-success">True</span> : <span className="text-muted">False</span>}
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                {!this.state.isComplete && <button className="btn btn-primary mr-1" onClick={this.loadLighthouses}>Load more</button>}
            </div>;
        }
        return <p>No lighthouses</p>
    }

    render() {
        return (
            <div>
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
                                    {lighthouse.enabled ? <span className="text-success">True</span> : <span className="text-muted">False</span>}
                                </td>
                                <td>
                                    {lighthouse.hasError ? <span className="text-danger">True</span> : <span className="text-success">False</span>}
                                </td>
                                <td>
                                    {lighthouse.subscribed ? <span className="text-success">True</span> : <span className="text-muted">False</span>}
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                {!this.state.isComplete && <button className="btn btn-primary mr-1" onClick={this.loadLighthouses.bind(this)}>Load more</button>}
            </div>      
    );
  }
}
