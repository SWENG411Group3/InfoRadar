import React, { Component } from "react";
import apiService from "../services/ApiService";

function readJsonFile(e) {
    return new Promise((resolve, reject) => {
        const { target } = e;

        const file = target.files[0];
        const reader = new FileReader();

        reader.addEventListener("load", e => {
            console.log("out: ", reader.result);
            try {
                resolve(JSON.parse(reader.result.toString()));
            } catch (e) {
                reject(e);
            }
            target.value = null;
        });

        const err = e => {
            target.value = null;
            reject(e);
        };

        reader.addEventListener("abort", err);
        reader.addEventListener("error", err);
        reader.readAsText(file);
    });
}

export class DashboardAdminButtons extends Component {
    static displayName = DashboardAdminButtons.name;

    constructor(props) {
        super(props);
        this.templates = {};
        this.state = {
            canTemplate: false,
        }
    }

    async uploadTemplate(e) {
        readJsonFile(e)
            .then(json => {
                apiService.uploadTemplate(json)
                    .then(liveTemplate => {
                        json.id = liveTemplate.id;
                        this.templates[json.internalName] = json;
                        this.setState({
                            canTemplate: true,
                        });
                        alert("Template uploaded");
                    })
                    .catch(e => {
                        console.error(e);
                        alert("Error: " + e);
                    })
            })
            .catch(e => {
                alert("Could not parse template. Template body must be a valid JSON");
                console.error(e);
            });
    }

    async newLighthouse(e) {}

    async templLighthouse(e) {}

    render() {
        return (
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
                        accept=".json" onInput={this.uploadTemplate.bind(this)} />
                    <input id="upload-custom-lighthouse" 
                        className="invisible"
                        type="file" name="new-lighthouse-file" 
                        accept=".json" onInput={this.newLighthouse} />
                    <input id="upload-template-lighthouse" 
                        className="invisible"
                        type="file" name="new-lighthouse-template-file" 
                        accept=".json" onInput={this.templLighthouse} />
                </div>
            </div>
        );
    }
}