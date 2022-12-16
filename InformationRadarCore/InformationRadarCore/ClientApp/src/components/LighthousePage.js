import React, { Component } from "react";
import apiService from "../services/ApiService";
import { useParams } from "react-router-dom";
import { Reports } from "./Reports";

function formatSeconds(seconds) {
    seconds = +seconds;
    const hours = Math.floor(seconds / 3600);
    seconds %= 3600;

    const minutes = Math.floor(seconds / 60);
    seconds %= 60;

    if (hours > 0) {
        return `${hours}h ${minutes}m ${seconds}s`;
    }

    if (minutes > 0) {
        return `${minutes}m ${seconds}s`;
    }

    return seconds + "s";
}

class LighthouseBody extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loading: true,
        };
    }

    componentDidMount() {
        this.loadLighthouse();
    }

    async loadLighthouse() {
        console.log("Loading lighthouse " + this.props.id);
        const lighthouse = await apiService.getLighthouse(this.props.id);
        const isAdmin = await apiService.isAdmin();
  
        this.setState({
            loading: false,
            lighthouse,
            isAdmin,
        });
    }

    tagline() {
        if (this.state.lighthouse.description) {
            return <p>{this.state.lighthouse.description}</p>
        }
        return <p>Lighthouse <code>{this.state.lighthouse.internalName}</code></p>
    }

    onEnable(e) {
        apiService.enableLighthouse(this.props.id, e.target.checked)
    }

    onSubscribe(e) {
        apiService.subscribe(this.props.id, e.target.checked)
    }

    onResolve(e) {
        if (e.target.checked) {
            apiService.resolve(this.props.id);
        }
    }

    render() {
        if (this.state.loading) {
            return <div>Loading lighthouse...</div>
        } else {
            return (
                <div>

                    <div className="row">
                        <div className="col">
                            <h1>{this.state.lighthouse.title}</h1>
                            {this.tagline()}

                            <table className="table table-striped" aria-labelledby="tabelLabel">
                                <tbody>
                                    <tr>
                                        <td><label htmlFor="lighthouse-enabled-check">Enabled</label></td>
                                        <td>
                                            <input type="checkbox"
                                                id="lighthouse-enabled-check"
                                                disabled={!this.state.isAdmin}
                                                defaultChecked={this.state.lighthouse.enabled}
                                                onChange={this.onEnable.bind(this)}
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><label htmlFor="lighthouse-error-check">Has Error</label></td>
                                        <td>
                                            <input type="checkbox"
                                                id="lighthouse-error-check"
                                                disabled={!this.state.isAdmin || !this.state.lighthouse.hasError}
                                                defaultChecked={this.state.lighthouse.hasError}
                                                onChange={this.onResolve.bind(this)}
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><label htmlFor="lighthouse-subscribed-check">Subscribed</label></td>
                                        <td>
                                            <input type="checkbox"
                                                id="lighthouse-subscribed-check"
                                                defaultChecked={this.state.lighthouse.subscribed}
                                                onChange={this.onSubscribe.bind(this)}
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Scrape period</td>
                                        <td>
                                            {formatSeconds(this.state.lighthouse.frequency)}
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Messenger period</td>
                                        <td>
                                            {formatSeconds(this.state.lighthouse.messengerFrequency || this.state.lighthouse.frequency)}
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <div className="col">
                            <Reports
                                lighthouse={this.state.lighthouse.id}
                                internalName={this.state.lighthouse.internalName}
                                isAdmin={this.state.isAdmin}
                            />
                        </div>
                    </div>
                    

                </div>
            );
        }        
  }
}

export function LighthousePage() {
    const { id } = useParams();
    return <LighthouseBody id={id} />
};