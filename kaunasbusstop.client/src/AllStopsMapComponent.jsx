import React, { useState, useEffect, useRef } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import "leaflet/dist/leaflet.css";

const AllStopsMapComponent = ({ coordinates, searchItem, selectedStopIndex, onMarkerClick }) => {

    const mapRef = useRef(null);
    const markersRef = useRef([]);

    const customIcon = L.icon({
        iconUrl: "/record-button.png",
        iconSize: [15, 15],
    });

    const filteredStops = searchItem
        ? coordinates.filter(stop => stop.stopName.toLowerCase().includes(searchItem.toLowerCase()))
        : coordinates;

    useEffect(() => {
        if (filteredStops.length > 0 && mapRef.current) {
            const { stopLat, stopLon } = filteredStops[0];
            mapRef.current.setView([stopLat, stopLon], 13);
        }
    }, [filteredStops]);

    useEffect(() => {
        if (selectedStopIndex !== null && markersRef.current[selectedStopIndex]) {
            setTimeout(() => {
                markersRef.current[selectedStopIndex].openPopup();
            }, 10);
        }
    }, [selectedStopIndex]);

    return (
        <MapContainer
            center={[54.8905, 23.927]}
            zoom={13}
            style={{ height: "90vh", width: "100%" }}
            whenCreated={(map) => (mapRef.current = map)}
        >
            <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
            {filteredStops.map((coord, index) => (
                <Marker
                    key={index}
                    position={[coord.stopLat, coord.stopLon]}
                    icon={customIcon}
                    ref={(el) => (markersRef.current[index] = el)}
                    eventHandlers={{
                        click: () => onMarkerClick(index),
                    }}
                >
                    <Popup><strong>Stotelė:</strong> {coord.stopName}</Popup>
                </Marker>
            ))}
        </MapContainer>
    );
};

export default AllStopsMapComponent;