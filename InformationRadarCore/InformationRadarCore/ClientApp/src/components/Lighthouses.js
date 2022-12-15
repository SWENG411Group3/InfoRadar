import React, { Component } from "react";
import authService from "./api-authorization/AuthorizeService";
import apiService from "../services/ApiService";

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
        this.templates = {};
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

    async loadTemplates() {}

    static readJsonFile(e) {
        return new Promise((resolve, reject) => {
            const file = e.target.files[0];
            const reader = new FileReader();
            reader.addEventListener("load", e => {
                try {
                    resolve(JSON.parse(reader.result.toString()));
                } catch (e) {
                    reject(e);
                }
            });
            reader.addEventListener("abort", reject);
            reader.addEventListener("error", reject);
            reader.readAsText(file);
        });
    }

    async uploadTemplate(e) {
        let template = {};
        try {
            template = await Lighthouses.readJsonFile(e);
        } catch (e) {
            alert("Could not parse template");
            console.error(e);
            return;
        }

        
    }

    async newLighthouse(e) {}

    async templLighthouse(e) {}

    render() {
        return (
            <div>
                {this.state.isAdmin && 
                    <div>
                        <div id="admin-upload">
                            <label htmlFor="upload-template">
                                <div className="btn btn-primary">Upload Template</div>
                            </label>

                            <label htmlFor="upload-custom-lighthouse">
                                <div className="btn btn-primary">New Custom Lighthouse</div>
                            </label>
                            
                            <label htmlFor="upload-template-lighthouse">
                                <div className="btn btn-primary">New Templated Lighthouse</div>
                            </label>
                        </div>
                        <div className="invisible">
                            <input id="upload-template" 
                                className="invisible"
                                type="file" name="template-file" 
                                accept=".json" onChange={this.uploadTemplate} />
                            <input id="upload-custom-lighthouse" 
                                className="invisible"
                                type="file" name="new-lighthouse-file" 
                                accept=".json" onChange={this.newLighthouse} />
                            <input id="upload-template-lighthouse" 
                                className="invisible"
                                type="file" name="new-lighthouse-template-file" 
                                accept=".json" onChange={this.templLighthouse} />
                        </div>
                    </div>}

                <table className='table table-striped' aria-labelledby="tabelLabel">
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
                {!this.state.isComplete && <button className="btn btn-primary" onClick={this.loadLighthouses}>Load more</button>}
            </div>      
    );
  }
}
