import React, { Component } from "react";

class UnsizedColumns extends Component {
    static Types = [
        "Int",
        "Long",
        "Small",
        "Real",
        "Text",
        "Date",
    ];

    constructor(props) {
        super(props);
        this.state = {
            nextType: null,
            nextCol: null,
            takenNames: [],
        };
    } 

    nextSelect(e) {
        this.setState({
            nextType: e.target.value,
            nextCol: this.state.nextCol,
            takenNames: this.state.takenNames,
        })
    }



    render() {
        return (
            <div className="add-unsized-column">
                <form>
                    <select onChange={this.nextSelect.bind(this)}>
                        {UnsizedColumns.Types.map(type => <option value={type}>{type}</option>)}
                    </select>

                </form>
            </div>
        );
    }

}