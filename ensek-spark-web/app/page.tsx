"use client";

import { useState, useEffect } from 'react';

const API_URL = process.env.NEXT_PUBLIC_API_URL!;

interface MeterReading {
  accountId: string;
  meterReadingDateTime: Date;
  meterReadValue: number;
}

const Home = () => {
  const [readings, setReadings] = useState<MeterReading[]>([]);
  const [file, setFile] = useState(null);
  const [error, setError] = useState('');
  const [uploadMessage, setUploadMessage] = useState('');

  // Fetch meter readings from the API
  useEffect(() => {
    const fetchMeterReadings = async () => {
      try {
        const response = await fetch(API_URL + '/meter-readings');
        if (response.ok) {
          const data = await response.json();
          setReadings(data);
        } else {
          setError('Failed to load meter readings.');
        }
      } catch (error) {
        setError('Error fetching data.');
      }
    };

    fetchMeterReadings();
  }, []);

  // Handle file selection
  const handleFileChange = (event: any) => {
    setFile(event.target.files[0]);
  };

  // Handle file upload
  const handleUpload = async () => {
    if (!file) {
      setUploadMessage('Please select a file to upload.');
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch(API_URL + '/meter-reading-uploads', {
        method: 'POST',
        body: formData,
      });

      if (response.ok) {
        const result = await response.json();
        setUploadMessage(`Successfully uploaded: ${result.successfulReadingsCount} readings`);
      } else {
        setUploadMessage('Failed to upload readings.');
      }
    } catch (error) {
      setUploadMessage('Error uploading file.');
    }
  };

  return (
    <div>
      <h1>Meter Readings</h1>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      {readings.length === 0 ? (
        <p>No meter readings found.</p>
      ) : (
        <ul>
          {readings.map((reading, index) => (
            <li key={index}>{`Account ID: ${reading.accountId}, Timestamp: ${reading.meterReadingDateTime}, Value: ${reading.meterReadValue}`}</li>
          ))}
        </ul>
      )}

      <div>
        <h2>Upload Meter Readings</h2>
        <input type="file" onChange={handleFileChange} />
        <button onClick={handleUpload}>Upload</button>
        {uploadMessage && <p>{uploadMessage}</p>}
      </div>
    </div>
  );
};

export default Home;
