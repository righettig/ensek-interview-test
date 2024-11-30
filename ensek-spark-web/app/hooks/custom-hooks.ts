import { useState, useEffect } from "react";

import MeterReading from "../models/meter-reading.model";

const API_URL = process.env.NEXT_PUBLIC_API_URL!;

// Custom hook for fetching meter readings
export const useMeterReadings = () => {
    const [readings, setReadings] = useState<MeterReading[]>([]);
    const [error, setError] = useState<string>("");

    useEffect(() => {
        const fetchMeterReadings = async () => {
            try {
                const response = await fetch(`${API_URL}/meter-readings`);
                if (response.ok) {
                    const data = await response.json();
                    setReadings(data);
                } else {
                    setError("Failed to load meter readings.");
                }
            } catch (err) {
                setError("Error fetching data.");
            }
        };

        fetchMeterReadings();
    }, []);

    return { readings, error };
};

// Custom hook for uploading files
export const useFileUploader = () => {
    const [uploadMessage, setUploadMessage] = useState<string>("");

    const handleUpload = async (file: File | null) => {
        if (!file) {
            setUploadMessage("Please select a file to upload.");
            return;
        }

        const formData = new FormData();
        formData.append("file", file);

        try {
            const response = await fetch(`${API_URL}/meter-reading-uploads`, {
                method: "POST",
                body: formData,
            });

            if (response.ok) {
                const result = await response.json();
                setUploadMessage(
                    `Upload response - successfulCount: ${result.successfulCount}, failedCount: ${result.failedCount}`
                );
            } else {
                setUploadMessage("Failed to upload readings.");
            }
        } catch (err) {
            setUploadMessage("Error uploading file.");
        }
    };

    return { uploadMessage, handleUpload };
};
