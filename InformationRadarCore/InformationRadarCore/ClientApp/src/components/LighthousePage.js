import React, { Component } from "react";
import authService from "./api-authorization/AuthorizeService";
import apiService from "../services/ApiService";
import { useParams } from "react-router-dom";

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

    render() {
        if (this.state.loading) {
            return <div>Loading lighthouse...</div>
        } else {
            return (
                <div>
                    <h1>{this.state.lighthouse.title}</h1>
                    {this.tagline()}
                </div>
            );
        }        
  }
}

export function LighthousePage() {
    const { id } = useParams();
    return <LighthouseBody id={id} />
};