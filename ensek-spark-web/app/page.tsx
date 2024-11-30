"use client";

import { useState, useEffect } from "react";

const API_URL = process.env.NEXT_PUBLIC_API_URL!;

interface MeterReading {
  accountId: string;
  meterReadingDateTime: string;
  meterReadValue: number;
}

// Custom hook for fetching meter readings
const useMeterReadings = () => {
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
const useFileUploader = () => {
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

// Helper function to format date
const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return new Intl.DateTimeFormat("en-GB", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  }).format(date);
};

const FileUploader = ({ onUpload }: { onUpload: (file: File | null) => void }) => {
  const [file, setFile] = useState<File | null>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFile(event.target.files?.[0] || null);
  };

  return (
    <div className="file-uploader">
      <h2>Upload Meter Readings</h2>
      <input type="file" onChange={handleFileChange} />
      <button onClick={() => onUpload(file)}>Upload</button>
    </div>
  );
};

const MeterReadingsList = ({ readings }: { readings: MeterReading[] }) => {
  if (readings.length === 0) {
    return <p>No meter readings found.</p>;
  }

  return (
    <ul>
      {readings.map((reading, index) => (
        <li key={index}>
          {`Account ID: ${reading.accountId}, Timestamp: ${formatDate(reading.meterReadingDateTime)}, Value: ${reading.meterReadValue}`}
        </li>
      ))}
    </ul>
  );
};

const Home = () => {
  const { readings, error } = useMeterReadings();
  const { uploadMessage, handleUpload } = useFileUploader();

  return (
    <div className="container">
      <h1>ENSEK Spark</h1>
      <h2>Meter Readings</h2>

      <FileUploader onUpload={handleUpload} />
      {uploadMessage && <p className="message">{uploadMessage}</p>}

      <div className="readings-section">
        {error && <p className="error">{error}</p>}
        <MeterReadingsList readings={readings} />
      </div>
    </div>
  );
};

export default Home;
