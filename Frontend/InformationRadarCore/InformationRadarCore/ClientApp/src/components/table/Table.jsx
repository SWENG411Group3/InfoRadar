import "./table.scss";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import Chip from '@mui/material/Chip';

import Stack from '@mui/material/Stack';
import ReportProblemRoundedIcon from '@mui/icons-material/ReportProblemRounded';
import WindPowerRoundedIcon from '@mui/icons-material/WindPowerRounded';
import CloudQueueRoundedIcon from '@mui/icons-material/CloudQueueRounded';
import BoltRoundedIcon from '@mui/icons-material/BoltRounded';
import AirplaneTicketRoundedIcon from '@mui/icons-material/AirplaneTicketRounded';
import ApiRoundedIcon from '@mui/icons-material/ApiRounded';

import Tooltip from '@mui/material/Tooltip';
import { Typography } from "@mui/material";





const List = () => {
    const rows = [
        {
            id:
                <Tooltip title={<Typography>
                    <b>{'The Nuclear Tag:'}</b> {"This technology utilizes Nuclear Energy."} <u>{'Nuclear Energy'}</u> {"is the energy in the nucleus, or core, of an atom. We can use this to create electricity, but it must first be released from the atom."}
                </Typography>}>
                    <Stack direction="column" spacing={1}>
                        <Chip color="error" label="Nuclear" icon={<ReportProblemRoundedIcon />} />
                    </Stack>
                </Tooltip>,
            product: "Kiran Narcisse",
            img: "https://m.media-amazon.com/images/I/81bc8mA3nKL._AC_UY327_FMwebp_QL65_.jpg",
            customer: "Small Scale Nuclear",
            date: "Very interesting new technology, should loop in person x and person y for a meeting to discuss project Z and incorporating Small Scale Nuclar",
            amount: 'Jun 17, 2022',
            method: "-----",
            status: "Scored",
        },
        {
            id: <Tooltip title={<Typography>
                <b>{'The Air Tag:'}</b> {"This technology utilizes Air."} <u>{'Wind'}</u> {"energy is energy from moving air. Air has mass. When it moves, it has kinetic energy."}
                <b>{'The CO2 Tag:'}</b> {"This technology utilizes Carbon Dioxide."} <u>{'CO2:'}</u> {"is able to interact with infrared radiation, leading to an imbalance of radiation entering and leaving the atmosphere. Carbon dioxide (CO2) is a naturally occurring gas, important to the carbon cycle for life and a byproduct of many forms of energy production. It is also a greenhouse gas."}
            </Typography>}>

                <Stack direction="column" spacing={1}>
                    <Chip color="info" label="Air" icon={<WindPowerRoundedIcon />} />
                    <Chip color="warning" label="CO2" icon={<CloudQueueRoundedIcon />} />
                </Stack>
            </Tooltip>
            ,
            product: "Goda Joel",
            img: "https://m.media-amazon.com/images/I/31JaiPXYI8L._AC_UY327_FMwebp_QL65_.jpg",
            customer: "Direct Air CO2 Capture",
            date: "New Research gave us more of a reason to lower the Non-Utlization score by a margin of 2",
            amount: 'Jun 12, 2022',
            method: "-----",
            status: "Pending",
        },
        {
            id: <Tooltip title={<Typography>
                <b>{'The Nickel Tag:'}</b> {"This technology utilizes Nickel."} <u>{'Nickel:'}</u> {"A nickel metal hydride battery (NiMH or Ni–MH) is a type of rechargeable battery. The chemical reaction at the positive electrode is similar to that of the nickel–cadmium cell (NiCd), with both using nickel oxide hydroxide (NiOOH). However, the negative electrodes use a hydrogen-absorbing alloy instead of cadmium."}
            </Typography>}>

                <Stack direction="column" spacing={1}>
                    <Chip color="secondary" label="Nickel" icon={<BoltRoundedIcon />} />
                </Stack>
            </Tooltip>



            ,
            product: "Kara Eva",
            img: "https://m.media-amazon.com/images/I/71kr3WAj1FL._AC_UY327_FMwebp_QL65_.jpg",
            customer: "High Nickel NMC",
            date: "We decided to lower the Robustness score of Space Based Solar because of new information that was shared with us at X conference",
            amount: 'May 20, 2022',
            method: "-----",
            status: "Pending",
        },
        {
            id: <Tooltip title={<Typography>
                <b>{'The Drone Tag:'}</b> {"This technology utilizes Drone Technology."} <u>{'Drones:'}</u> {"combine several complimentary technologies on a single platform. A single drone can contain highly advanced surveillance systems, live-feed video cameras, infrared cameras, thermal sensors and radar, and various types of other equipment including global positioning systems (GPS), and precision munitions"}
            </Typography>}>

                <Stack direction="column" spacing={1}>
                    <Chip color="success" label="Drone" icon={<AirplaneTicketRoundedIcon />} />
                </Stack>
            </Tooltip>



            ,
            product: "Rufus Skadi",
            img: "https://m.media-amazon.com/images/I/71wF7YDIQkL._AC_UY327_FMwebp_QL65_.jpg",
            customer: "Drone Fire Suppression",
            date: "Please read article attached when you get the chance ... has valuable info that could change the score ",
            amount: 'April 22, 2022',
            method: "-----",
            status: "Scored",
        },
        {
            id: <Tooltip title={<Typography>
                <b>{'The Iron Air Tag:'}</b> {"This technology utilizes Iron Air."} <u>{'Iron Air batteries'}</u> {"are an electrochemical cell that uses an anode made from pure metal and an external cathode of ambient air, typically with an aqueous or aprotic electrolyte."}

            </Typography>}>

                <Stack direction="column" spacing={1}>
                    <Chip color="info" label="Air" icon={<WindPowerRoundedIcon />} />
                    <Chip color="default" label="Iron" icon={<ApiRoundedIcon />} />
                </Stack>
            </Tooltip>,
            product: "Theodore Lhamo",
            img: "https://m.media-amazon.com/images/I/81hH5vK-MCL._AC_UY327_FMwebp_QL65_.jpg",
            customer: "Iron Air",
            date: "Worth a review ASAP when possible, brings up new technologies that can affect X,Y,Z",
            amount: 'Apr 2, 2022',
            method: "----- ",
            status: "Pending",
        },
    ];
    return (
        <TableContainer component={Paper} className="table">
            <Table sx={{ minWidth: 650 }} aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell className="tableCell">Tags</TableCell>
                        <TableCell className="tableCell">Contributor</TableCell>
                        <TableCell className="tableCell">Technology</TableCell>
                        <TableCell className="tableCell">Notes</TableCell>
                        <TableCell className="tableCell">Date Submitted</TableCell>
                        <TableCell className="tableCell">Test</TableCell>
                        <TableCell className="tableCell">Status</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {rows.map((row) => (
                        <TableRow key={row.id}>
                            <TableCell className="tableCell">{row.id}</TableCell>
                            <TableCell className="tableCell">
                                <div className="cellWrapper">
                                    <img src={row.img} alt="" className="image" />
                                    {row.product}
                                </div>
                            </TableCell>
                            <TableCell className="tableCell">{row.customer}</TableCell>
                            <TableCell className="tableCell">{row.date}</TableCell>
                            <TableCell className="tableCell">{row.amount}</TableCell>
                            <TableCell className="tableCell">{row.method}</TableCell>
                            <TableCell className="tableCell">
                                <span className={`status ${row.status}`}>{row.status}</span>
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default List;