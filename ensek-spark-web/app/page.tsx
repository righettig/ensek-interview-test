"use client";

import { useState, useEffect } from 'react';

const API_URL = process.env.NEXT_PUBLIC_API_URL!;

interface MeterReading {
  accountId: string;
  meterReadingDateTime: string;
  meterReadValue: number;
}

const Home = () => {
  const [readings, setReadings] = useState<MeterReading[]>([]);
  const [file, setFile] = useState(null);
  const [error, setError] = useState('');
  const [uploadMessage, setUploadMessage] = useState('');

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false, // Use 24-hour format
    }).format(date);
  };

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
        setUploadMessage(`Upload response - successfulCount: ${result.successfulCount}, failedCount: ${result.failedCount}`);
      } else {
        setUploadMessage('Failed to upload readings.');
      }
    } catch (error) {
      setUploadMessage('Error uploading file.');
    }
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <h1 style={{ fontSize: '2em' }}>ENSEK Spark</h1>
      <h2 style={{ fontSize: '1.2em', marginTop: '12px' }}>Meter Readings</h2>

      <div style={{ marginTop: '12px' }}>
        <h2 style={{ marginBottom: '8px' }}>Upload Meter Readings</h2>
        <input type="file" onChange={handleFileChange} />
        <button onClick={handleUpload}>Upload</button>
        <div style={{ marginTop: '8px' }}>
          {uploadMessage && <p>{uploadMessage}</p>}
        </div>
      </div>

      <div style={{ padding: '24px', marginTop: '12px', border: 'solid 1px black' }}>
        <div>
          {error && <p style={{ color: 'red' }}>{error}</p>}
        </div>

        {readings.length === 0 ? (
          <p>No meter readings found.</p>
        ) : (
          <ul>
            {readings.map((reading, index) => (
              <li key={index}>
                {`Account ID: ${reading.accountId}, Timestamp: ${formatDate(reading.meterReadingDateTime)}, Value: ${reading.meterReadValue}`}
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default Home;
