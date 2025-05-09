import React, { useState, useEffect } from "react";
import axios from "axios";
import { Link } from "react-router-dom";
import AllStopsMapComponent from "../AllStopsMapComponent.jsx";

function Home() {
    const [allRoutesData, setAllRoutesData] = useState(null);
    const [allStopsData, setAllStopsData] = useState(null);
    const [filteredTypes, setFilteredTypes] = useState(null);
    const [routeTypeName, SetRouteTypeName] = useState(null);
    const [searchItem, setSearchItem] = useState("");
    const [searchResult, setSearchResult] = useState(null);
    const [stopData, setStopData] = useState(null);
    const [selectedStopIndex, setSelectedStopIndex] = useState(null);
    const [stopId, setStopId] = useState("");
    const displayData = searchItem ? searchResult : filteredTypes;

    useEffect(() => {
        handleAllRoutesData();
    }, []);

    useEffect(() => {
        handleApiSelectionStopId();
        //setStopId();
    }, [stopId]);

    const handleStopIdSelect = (e) => {  
       const stopIndex = e.target.value;  
       if (stopIndex !== "") {  
           const selectedStop = allStopsData.find((stop) => stop.stopId == stopIndex);    
           setStopId(selectedStop ? selectedStop.stopId : null);  
           setSelectedStopIndex(allStopsData.indexOf(selectedStop));
       } else {  
           setStopId(null);  
           setSelectedStopIndex(null);
       }  
    };

    const handleMarkerClick = (index) => {
        const selectedStop = allStopsData[index];
        setStopId(selectedStop.stopId);
        setSelectedStopIndex(index);
    };

    const handleInputChange = (e) => {
        const searchTerm = e.target.value;
        setSearchItem(searchTerm);

        if (allStopsData) {
            const searchResults = allStopsData.filter((stop) =>
                stop.stopName.toLowerCase().includes(searchTerm.toLowerCase())
            );
            setSearchResult(searchResults);
        } else {
            const searchResults = filteredTypes.filter(
                (route) =>
                    route.routeLongName
                        .toLowerCase()
                        .includes(searchTerm.toLowerCase()) ||
                    route.routeShorName.toLowerCase().includes(searchTerm.toLowerCase())
            );
            setSearchResult(searchResults);
        }
    };

    const handlesCityStopMap = () => {
        handleAllStopsData();
        setSearchItem("");
        setStopData("");
    };

    const handleAllTypes = () => {
        setAllStopsData(null);
        setFilteredTypes(allRoutesData);
        SetRouteTypeName("Autobusai ir Troleibusai");
        setSearchItem("");
    };

    const handleBusFileter = () => {
        setAllStopsData(null);
        const bus = allRoutesData.filter(
            (route) => route.routeType === "A"
        );
        setFilteredTypes(bus);
        SetRouteTypeName("Autobusai");
        setSearchItem("");
    };

    const handleTrolFileter = () => {
        setAllStopsData(null);
        const trol = allRoutesData.filter(
            (route) => route.routeType === "T"
        );
        setFilteredTypes(trol);
        SetRouteTypeName("Troleibusai");
        setSearchItem("");
    };

    const handleApiSelectionStopId = async () => {

        if (stopId) {
            await axios
                .get(`https://localhost:7115/api/busStop/v1/StopSchedule/GetAllStopSchedule?id=${stopId}`)
                .then(function (response) {
                    setStopData(response.data[0].stopInformation);
                })
                .catch(function (error) {
                    console.log(error);
                });
        }
    };

    const handleAllRoutesData = async () => {
        await axios
            .get(`https://localhost:7115/api/busStop/v1/RouteWorkWeek/GetAllRouteWorkWeek`)
            .then(function (response) {
                setAllRoutesData(response.data);
                setFilteredTypes(response.data);
                SetRouteTypeName("Autobusai ir Troleibusai");
            })
            .catch(function (error) {
                console.log(error);
            });
    };

    const handleAllStopsData = async () => {
        await axios
            .get(`https://localhost:7115/api/busStop/v1/GTFS/getAllStops`)
            .then(function (response) {
                setAllStopsData(response.data);
            })
            .catch(function (error) {
                console.log(error);
            });
    };

    return (
        <div className="container-fluid mt-5">
            {allRoutesData && (
                <div>
                    <div className="text-center ">
                        <div className="row ">
                            <div className="col ">
                                <div className="input-group mb-3 ">
                                    <span
                                        className="input-group-text"
                                        id="inputGroup-sizing-default"
                                    >
                                        Search
                                    </span>
                                    <input
                                        value={searchItem}
                                        type="text"
                                        className="form-control"
                                        aria-label="Sizing example input"
                                        aria-describedby="inputGroup-sizing-default"
                                        onChange={handleInputChange}
                                    />
                                </div>
                            </div>

                            <div className="col">
                                <div
                                    className="btn-group btn-group-lg text-nowrap"
                                    role="group"
                                    aria-label="Large button group"
                                >
                                    <input
                                        type="radio"
                                        className="btn-check"
                                        name="options"
                                        id="option1"
                                        autoComplete="off"
                                        defaultChecked
                                        onChange={handleAllTypes}
                                    />
                                    <label className="btn btn-outline-primary" htmlFor="option1">
                                        Autobusai ir Troleibusai
                                    </label>

                                    <input
                                        type="radio"
                                        className="btn-check"
                                        name="options"
                                        id="option2"
                                        autoComplete="off"
                                        onClick={handleBusFileter}
                                    />
                                    <label className="btn btn-outline-primary" htmlFor="option2">
                                        Autobusai
                                    </label>

                                    <input
                                        type="radio"
                                        className="btn-check"
                                        name="options"
                                        id="option3"
                                        autoComplete="off"
                                        onClick={handleTrolFileter}
                                    />
                                    <label className="btn btn-outline-primary" htmlFor="option3">
                                        Troleibusai
                                    </label>

                                    <input
                                        type="radio"
                                        className="btn-check"
                                        name="options"
                                        id="option4"
                                        autoComplete="off"
                                        onClick={handlesCityStopMap}
                                    />
                                    <label className="btn btn-outline-primary" htmlFor="option4">
                                        žemėlapis
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                    {allStopsData ? (
                        <div className="text-center">
                            <div className="row">
                                <div className="col-4">
                                    <select
                                        className="form-select form-select-sm"
                                        aria-label="Small select example"
                                        onChange={handleStopIdSelect}
                                    >
                                        <option selected>Open this select menu</option>
                                        {allStopsData.map((stop, index) => (
                                            <option key={index} value={stop.stopId} >
                                                {stop.stopName}
                                            </option>
                                        ))}
                                    </select>
                                    {stopData && (
                                    <div className="scrollable-list border p-2" style={{ maxHeight: "85vh", overflowY: "auto" }}>
                                        {stopData.map((stop, index) => (
                                            <div key={index} className="list-group-item border-bottom">
                                                {stop.routeShorName} {stop.routeLongName}
                                                <div>
                                                    {stop.workDays.map((day, dayIndex) => (
                                                        <span key={dayIndex} className="border p-1 m-1 d-inline-block">
                                                            {day}
                                                        </span>
                                                    ))}
                                                </div>
                                                <p>{stop.arrivalTime.join(" ")}</p>
                                            </div>
                                        ))}
                                    </div>
                                )}
                                    
                                </div>
                                <div className="col-8">
                                    <AllStopsMapComponent
                                        coordinates={allStopsData}
                                        searchItem={searchItem}
                                        selectedStopIndex={selectedStopIndex}
                                        onMarkerClick={handleMarkerClick}
                                    />
                                </div>
                            </div>
                        </div>
                    ) : (
                        displayData && (
                            <div>
                                <h3>{routeTypeName}</h3>
                                <table className="table">
                                    <tbody>
                                        {displayData.map((route, index) => (
                                            <tr key={index}>
                                                <td className="col-4">
                                                    {route.routeShorName}
                                                    <Link
                                                        className="col-4 p-3 link-dark hover-light link-offset-2 link-underline link-underline-opacity-0"
                                                        to="/RouteSchedule"
                                                        state={{ routeId: route.routeId }}
                                                    >
                                                        {route.routeLongName}
                                                    </Link>
                                                </td>
                                                <td className="col-4">{route.activeDays.join(" ")}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        )
                    )}
                </div>
            )}
        </div>
    );
}

export default Home;