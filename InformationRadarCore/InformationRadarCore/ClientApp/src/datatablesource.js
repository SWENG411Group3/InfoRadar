
export const userColumns = [
    { field: "id", headerName: "ID", width: 70 },
    {
        field: "email",
        headerName: "Lighthouse Size",
        width: 230,
    },
    {
        field: "title",
        headerName: "Topic",
        width: 430,
    },
    {
        field: "age",
        headerName: "Lighthouse Started On",
        width: 230,
    },
    {
        field: "status",
        headerName: "Status",
        width: 160,
        renderCell: (params) => {
            return (
                <div className={`cellWithStatus ${params.row.status}`}>
                    {params.row.status}
                </div>
            );
        },
    },
];

//temporary data
export const userRows = [
    {
        id: 1,
        username: "Timothy Effio",
        img: "https://images.pexels.com/photos/214574/pexels-photo-214574.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
        status: "Open",
        title: "Small Scale Nuclear",
        email: 'Small',
        age: 'December 21, 2021',
    },
    {
        id: 2,
        username: "Vasu Nambeesan",
        img: "https://images.pexels.com/photos/39853/woman-girl-freedom-happy-39853.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
        status: "Open",
        title: "Manufactuering Solar in Space",
        email: "Large",
        age: 'December 22, 2021',
    }, {
        id: 3,
        username: "Vasu Nambeesan",
        img: "https://images.pexels.com/photos/39853/woman-girl-freedom-happy-39853.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
        status: "Pending",
        title: "Graphene Nano Tech",
        email: "Extra Large",
        age: 'December 27, 2021',
    }, {
        id: 4,
        username: "Colleen Lueken",
        img: "https://images.pexels.com/photos/762041/pexels-photo-762041.jpeg?auto=compress&cs=tinysrgb&w=1600",
        status: "Pending",
        title: "Battery Analytics",
        email: "Medium",
        age: 'January 5, 2022',
    }, {
        id: 5,
        username: "TJ Winter",
        img: "https://images.pexels.com/photos/1583582/pexels-photo-1583582.jpeg?auto=compress&cs=tinysrgb&w=1600",
        status: "Pending",
        title: "Zinc-3DC aqueous battery",
        email: "Small",
        age: 'January 6, 2022',
    }, {
        id: 6,
        username: "Vasu Nambeesan",
        img: "https://images.pexels.com/photos/39853/woman-girl-freedom-happy-39853.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
        status: "Pending",
        title: "Li Ion Battery Recycling",
        email: "Large",
        age: 'January 6, 2022',
    }, {
        id: 7,
        username: "Colleen Lueken",
        img: "https://images.pexels.com/photos/762041/pexels-photo-762041.jpeg?auto=compress&cs=tinysrgb&w=1600",
        status: "Open",
        title: "Direct air CO2 capture",
        email: "Extra Large",
        age: 'January 10, 2022',
    }, {
        id: 8,
        username: "TJ Winter",
        img: "https://images.pexels.com/photos/1583582/pexels-photo-1583582.jpeg?auto=compress&cs=tinysrgb&w=1600",
        status: "Closed",
        title: "Sodium-Metal Anode Sodium-Ion Battery",
        email: "Small",
        age: 'January 11, 2022',
    }, {
        id: 9,
        username: "Vasu Nambeesan",
        img: "https://images.pexels.com/photos/39853/woman-girl-freedom-happy-39853.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
        status: "Open",
        title: "V2G, BESS Bidding and optimization",
        email: "Small",
        age: 'January 11, 2022',
    }, {
        id: 10,
        username: "TJ Winter",
        img: "https://images.pexels.com/photos/1583582/pexels-photo-1583582.jpeg?auto=compress&cs=tinysrgb&w=1600",
        status: "Pending",
        title: "Ultrasound Based Battery",
        email: "Large",
        age: 'January 13, 2022',
    },


];