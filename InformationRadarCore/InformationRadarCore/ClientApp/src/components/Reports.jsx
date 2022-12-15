import React, { Component } from "react";
import apiService from "../services/ApiService";


class NewReport extends Component {
    constructor(props) {
        super(props);
        this.state = {
            pages: null,
            recordsPerPage: null,
            isJson: true,
        };
    }

    pageUpdate(e) {
        this.setState({
            pages: parseInt(e.target.value, 10) || null,
            recordsPerPage: this.state.recordsPerPage,
            isJson: this.state.isJson,
        });
    }

    formatUpdate(e) {
        this.setState({
            pages: this.state.pages,
            recordsPerPage: this.state.recordsPerPage,
            isJson: e.target.value === "Json",
        });
    }

    recordsUpdate(e) {
        this.setState({
            pages: this.state.pages,
            recordsPerPage: parseInt(e.target.value, 10) || null,
            isJson: this.state.isJson,
        });
    }

    submitForm() {
        const succ = this.props.onCreate;
        apiService.generateReport(
            this.props.lighthouse,
            this.state.isJson ? "json" : "csv",
            this.state.recordsPerPage,
            this.state.pages,
        )
            .then(e => {
                if (succ) {
                    succ({
                        fileName: e.fileName,
                        created: Date.now(),
                        webPath: `/Reports/${this.props.internalName}/${e.fileName}`,
                        reportType: this.state.isJson ? "Json" : "Csv",
                    });
                }
            }).catch(e => {
                console.error(e);
                alert("Could not generate report");
            });
    }

    render() {
        return (
            <div>
                <h4>New Report</h4>
                <input type="number" placeholder="Pages" min="1" onChange={this.pageUpdate.bind(this)} />
                <input type="number" placeholder="R/P" min="1" onChange={this.recordsUpdate.bind(this)} />
                <select onChange={this.formatUpdate.bind(this)}>
                    <option value="Json">JSON</option>
                    <option value="Csv">CSV</option>
                </select>
                <button onClick={this.submitForm.bind(this)}>Generate Report</button>
            </div>
        );
    }
}

export class Reports extends Component {
    constructor(props) {
        super(props);
        this.state = {
            entries: [],
            isComplete: true,
            cursor: null,
        };
    }

    componentDidMount() {
        this.loadReports();
    }

    async loadReports() {
        const { entries, isComplete, cursor } = await apiService.getReports({
            lighthouse: this.props.lighthouse
        });

        this.setState({
            entries: this.state.entries.concat(entries),
            isComplete,
            cursor,
        });
    }

    reportCreated(report) {
        this.setState({
            entries: [report, ...this.state.entries],
            isComplete: this.state.isComplete,
            cursor: this.state.cursor,
        });
    }

    body() {
        if (this.state.entries.length > 0) {
            return <table className="table table-striped" aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Created</th>
                        <th>Type</th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.entries.map(e => <tr>
                        <td><a href={e.webPath} download>{e.fileName}</a></td>
                        <td>{new Date(e.created).toLocaleString()}</td>
                        <td>{e.reportType}</td>
                    </tr>)}
                </tbody>
            </table>;
        }

        return <p>No reports generated yet</p>
    }

    render() {
        return (
            <div>
                <h3>Reports</h3>
                {this.body()}
                {!this.state.isComplete && <button className="btn btn-primary mr-1" onClick={this.loadReports}>Load more</button>}
                {this.props.isAdmin && <NewReport
                    lighthouse={this.props.lighthouse}
                    onCreate={this.reportCreated.bind(this)}
                    internalName={this.props.internalName} />}
            </div>
        );
    }

}