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

    async uploadTemplate(e) {
        readJsonFile(e)
            .then(json => {
                apiService.uploadTemplate(json)
                    .then(liveTemplate => {
                        alert("Template uploaded");
                        window.location.reload();
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

    async newLighthouse(e) {
        const evnt = this.props.onNewLighthouse;
        readJsonFile(e)
            .then(json => {
                apiService.uploadCustomLighthouse(json)
                    .then(liveLighthouse => {
                        json.id = liveLighthouse.id;
                        json.subscribed = false;
                        if (evnt) {
                            evnt(json);
                        }
                        alert("Lighthouse uploaded");
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

    async templLighthouse(e) {
        const evnt = this.props.onNewLighthouse;
        readJsonFile(e)
            .then(json => {
                apiService.uploadTemplateLighthouse(json)
                    .then(liveLighthouse => {
                        json.id = liveLighthouse.id;
                        json.subscribed = false;
                        if (evnt) {
                            evnt(json);
                        }
                        alert("Lighthouse uploaded");
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
                        accept=".json" onInput={this.newLighthouse.bind(this)} />
                    <input id="upload-template-lighthouse" 
                        className="invisible"
                        type="file" name="new-lighthouse-template-file" 
                        accept=".json" onInput={this.templLighthouse.bind(this)} />
                </div>
            </div>
        );
    }
}