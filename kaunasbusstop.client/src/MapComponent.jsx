import React, { useState, useEffect, useRef } from "react";
import { MapContainer, TileLayer, Marker, Popup, useMap, Polyline } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import L from "leaflet";

const ChangeView = ({ center }) => {
    const map = useMap();
    map.setView(center);
    return null;
};

const fetchRoute = async (coordinates) => {
    if (coordinates.length < 2) return null;

    const coords = coordinates.map(coord => `${coord.stopLon},${coord.stopLat}`).join(";");
    const url = `https://router.project-osrm.org/route/v1/driving/${coords}?overview=full&steps=true`;

    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error("Failed to fetch route");
        }
        const data = await response.json();
        if (!data.routes || data.routes.length === 0) {
            throw new Error("No routes found");
        }
        return decodePolyline(data.routes[0].geometry);
    } catch (error) {
        console.error("Error fetching route:", error);
        return null;
    }
};

const MapComponent = ({ coordinates, selectedIndex, onMarkerClick }) => {
    const [center, setCenter] = useState([0, 0]);
    const markersRef = useRef([]);
    const [mapLoaded, setMapLoaded] = useState(false);
    const [route, setRoute] = useState(null);

    const customIcon = L.icon({
        iconUrl: "/record-button.png",
        iconSize: [15, 15],
    });

    useEffect(() => {
        if (coordinates.length > 0) {
            const middleIndex = Math.floor(coordinates.length / 2);
            setCenter([coordinates[middleIndex].stopLat, coordinates[middleIndex].stopLon]);
        }
    }, [coordinates]);

    useEffect(() => {
        if (mapLoaded && selectedIndex !== null && markersRef.current[selectedIndex]) {
            setTimeout(() => {
                markersRef.current[selectedIndex].openPopup();
            }, 10);
        }
    }, [mapLoaded, selectedIndex]);

    useEffect(() => {
        if (coordinates.length > 1) {
            fetchRoute(coordinates).then((polyline) => {
                setRoute(polyline);
            });
        }
    }, [coordinates]);

    return (
        <MapContainer
            center={center}
            zoom={13}
            style={{ height: "90vh", width: "100%" }}
            whenReady={() => setMapLoaded(true)}
        >
            <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />

            {route && <Polyline positions={route} color="green" weight={4} />}

            {coordinates.map((coord, index) => (
                <Marker
                    key={index}
                    position={[coord.stopLat, coord.stopLon]}
                    ref={(el) => (markersRef.current[index] = el)}
                    eventHandlers={{ click: () => onMarkerClick(index) }}
                    icon={customIcon}
                >
                    <Popup>{coord.stopName}</Popup>
                </Marker>
            ))}

            <ChangeView center={center} />
        </MapContainer>
    );
};

const decodePolyline = (encoded) => {
    let index = 0;
    const len = encoded.length;
    const points = [];
    let lat = 0;
    let lng = 0;

    while (index < len) {
        let b, shift = 0, result = 0;
        do {
            b = encoded.charCodeAt(index++) - 63;
            result |= (b & 0x1f) << shift;
            shift += 5;
        } while (b >= 0x20);
        const dlat = (result & 1) ? ~(result >> 1) : (result >> 1);
        lat += dlat;

        shift = 0;
        result = 0;
        do {
            b = encoded.charCodeAt(index++) - 63;
            result |= (b & 0x1f) << shift;
            shift += 5;
        } while (b >= 0x20);
        const dlng = (result & 1) ? ~(result >> 1) : (result >> 1);
        lng += dlng;

        points.push([lat / 1e5, lng / 1e5]);
    }

    return points;
};

export default MapComponent;