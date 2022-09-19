import "./home.scss";
import Sidebar from "../../components/sidebar/Sidebar";
import Navbar from "../../components/navbar/Navbar";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";

import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";

import Paper from "@mui/material/Paper";

import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';




import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import Tooltip from '@mui/material/Tooltip';
import { Typography } from "@mui/material";




const style = {
    top: '50%',
    right: 0,
    transform: 'translate(0, -50%)',
    lineHeight: '24px',
};


const Single = () => {




    const [open, setOpen] = React.useState(false);
    const handleClickOpen = () => {
        setOpen(true);
    }
    const handleClose = () => {
        setOpen(false);
    }

    return (
        <div className="single">
            <Sidebar />
            <div className="singleContainer">
                <Navbar />

                <div className="bottom">
                    <center>
                        <Button variant="outlined" onClick={handleClickOpen}>Add A lighthouse</Button>

                        <Dialog open={open} onClose={handleClose}>


                            <DialogTitle>Updating lighthouses</DialogTitle>
                            <DialogContent>
                                <DialogContentText>
                                    To add a lighthouse please fill out the form below, all fields are required
                                    <TextField
                                        autoFocus
                                        margin="dense"
                                        id="name"
                                        label="Name of LightHouse"
                                        type="Select"
                                        fullWidth
                                        variant="standard"
                                    />
                                    <TextField
                                        autoFocus
                                        margin="dense"
                                        id="name"
                                        label="Input Links Here Seperated By Commas"
                                        type="Select"
                                        fullWidth
                                        variant="standard"
                                    />

                                    <FormControl sx={{ m: 1, width: 300 }}>
                                        <InputLabel id="demo-select-small">Category</InputLabel>
                                        <Select
                                            labelId="demo-select-small"
                                            id="demo-select-small"

                                            label="Category"

                                        >
                                            <MenuItem value="">
                                                <em>None</em>
                                            </MenuItem>
                                            <MenuItem value={10}>Small</MenuItem>
                                            <MenuItem value={20}>Medium</MenuItem>
                                            <MenuItem value={30}>Large</MenuItem>
                                            <MenuItem value={40}>Extra Large</MenuItem>
                                        </Select>
                                    </FormControl>
                                    <FormControl sx={{ m: 1, width: 120 }}>
                                        <InputLabel id="demo-select-small">Status</InputLabel>
                                        <Select
                                            labelId="demo-select-small"
                                            id="demo-select-small"

                                            label="Status"

                                        >
                                            <MenuItem value="">
                                                <em>None</em>
                                            </MenuItem>
                                            <MenuItem value={10}>Open</MenuItem>
                                            <MenuItem value={20}>Closed</MenuItem>
                                        </Select>
                                    </FormControl>
                                </DialogContentText>
                            </DialogContent>
                            <DialogActions>
                                <Button onClick={handleClose}>Cancel</Button>
                                <Button onClick={handleClose}>Submit</Button>

                            </DialogActions>
                        </Dialog>
                    </center>
                    <h1 className="title">Light House Table</h1>
                    <Tooltip title={<Typography>
                        <b>{'light houses:'}</b>  {"are event factors that can be observed publicly which provide a signal that we should strongly consider"} <u>{"taking action"}</u>{" related to the technology."}
                    </Typography>}>
                        <center><Button variant="contained" >What is a lighthouse</Button></center>

                    </Tooltip>

                    <TableContainer component={Paper} className="table">
                        <Table sx={{ minWidth: 650 }} aria-label="simple table">
                            <TableHead>

                            </TableHead>
                            <TableBody>

                            </TableBody>
                        </Table>
                    </TableContainer>
                </div>
            </div>
        </div>
    );
};

export default Single;