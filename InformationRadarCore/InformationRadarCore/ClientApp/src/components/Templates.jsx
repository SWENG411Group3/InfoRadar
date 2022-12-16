import React, { Component } from "react";
import apiService from "../services/ApiService";
import { DashboardAdminButtons } from "./DashboardAdminButtons";

export class Templates extends Component {
    static displayName = Templates.name;

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
        this.loadTemplates();
    }

    async loadTemplates() {
        const old = this.state.entries;
        const templates = await apiService.getTemplates({
            cursor: this.state.cursor,
        });
        this.setState({
            entries: old.concat(templates.entries),
            isComplete: templates.isComplete,
            cursor: templates.cursor,
        });
    }

    render() {
        if (this.state.entries.length > 0) {
            return (<div className="container mt-3">
                <div className="row">
                    {this.state.entries.map(e =>
                        <div className="col-sm-3">
                            <h4 className="card-title">Template: {e.title}</h4>
                            <p className="card-description">Description: {e.description}</p>
                            <p className="card-description">Id: {e.id}</p>
                            <table className="table table-striped" aria-labelledby="tabelLabel">
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>JSON Data Type</th>
                                        <th>Description</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {e.fields.map(field => <tr>
                                        <td>{field.name}</td>
                                        <td>{field.dataType}</td>
                                        <td>{field.description}</td>
                                    </tr>)}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
                {!this.state.isComplete && <button className="btn btn-primary mr-1" onClick={this.loadTemplates.bind(this)}>Load more templates</button>}
            </div>);
        }
        return <p>No templates</p>
    }
}
