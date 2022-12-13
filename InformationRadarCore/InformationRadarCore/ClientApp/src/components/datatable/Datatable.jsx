import "./datatable.scss";
import { DataGrid } from "@mui/x-data-grid";
import { userColumns, userRows } from "../../datatablesource";
import { Link } from "react-router-dom";
import { useState } from "react";
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import * as React from 'react';


const Datatable = () => {
    const [data] = useState(userRows);
    const [open, setOpen] = React.useState(false);



    const handleClickOpen = () => {
        setOpen(true);
    }

    const handleClose = () => {
        setOpen(false);
    }

    const actionColumn = [
        {
            field: "action",
            headerName: "Action",
            width: 200,
            renderCell: (params) => {
                return (
                    <div className="cellAction">

                        <Link to={"/users/" + params.row.id} style={{ textDecoration: "none" }}>
                            <div className="viewButton">View</div>
                        </Link>

                        <div
                            className="deleteButton"
                            onClick={() => handleClickOpen(params.row.id)}
                        >
                            Delete
                        </div>

                    </div>
                );
            },
        },
    ];


    return (


        <div className="datatable">



            <div className="datatableTitle">

                Lighthouses
                <Link to="/users/new" className="link">
                    Add New
                </Link>

            </div>

            <DataGrid
                className="datagrid"
                rows={data}
                columns={userColumns.concat(actionColumn)}
                pageSize={50}
                rowsPerPageOptions={[50]}
                checkboxSelection
            />


            <Dialog
                open={open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">
                    {"You're about to purge a submission, are you sure?"}
                </DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        You will be permanently deleting this submission or selected submissions, are you sure you would like to continue?
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel
                    </Button>
                    <Button onClick={handleClose}>
                        Delete
                    </Button>
                </DialogActions>
            </Dialog>



        </div>

    );
};

export default Datatable;