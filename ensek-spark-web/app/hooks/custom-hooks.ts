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
interface UploadMeterReadingsResult {
    successfulCount: number;
    failedCount: number;
    failedDetails: string[] | null;
}

export const useFileUploader = () => {
    const [uploadMessage, setUploadMessage] = useState<string>("");
    const [failedDetails, setFailedDetails] = useState<string[] | null>(null);

    const handleUpload = async (file: File | null) => {
        if (!file) {
            setUploadMessage("Please select a file to upload.");
            setFailedDetails(null);
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
                const result: UploadMeterReadingsResult = await response.json();
                setUploadMessage(
                    `Upload response - successfulCount: ${result.successfulCount}, failedCount: ${result.failedCount}`
                );
                setFailedDetails(result.failedDetails);
            } else {
                setUploadMessage("Failed to upload readings.");
                setFailedDetails(null);
            }
        } catch (err) {
            setUploadMessage("Error uploading file.");
            setFailedDetails(null);
        }
    };

    return { uploadMessage, failedDetails, handleUpload };
};
