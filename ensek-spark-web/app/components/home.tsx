import { useState } from "react";
import { useFileUploader, useMeterReadings } from "../hooks/custom-hooks";

import FileUploader from "./file-uploader";
import MeterReadingsTable from "./meter-readings-table";

import styles from './home.module.css';

const Home = () => {
    const { readings, error } = useMeterReadings();
    const { uploadMessage, failedDetails, handleUpload } = useFileUploader();
    const [isExpanded, setIsExpanded] = useState<boolean>(false);

    return (
        <div className={styles.container}>
            <h1>ENSEK Spark</h1>
            <h2>Meter Readings</h2>

            <FileUploader onUpload={handleUpload} />
            {uploadMessage && <p className={styles.message}>{uploadMessage}</p>}

            {failedDetails && failedDetails.length > 0 && (
                <div className={styles['collapsible-section']}>
                    <button
                        className={styles['toggle-button']}
                        onClick={() => setIsExpanded(!isExpanded)}
                    >
                        {isExpanded ? "Hide Failed Details" : "Show Failed Details"}
                    </button>
                    {isExpanded && (
                        <ul className={styles['error-list']}>
                            {failedDetails.map((detail, index) => (
                                <li key={index}>{detail}</li>
                            ))}
                        </ul>
                    )}
                </div>
            )}

            {error && <p className={styles.error}>{error}</p>}

            <MeterReadingsTable readings={readings} />
        </div>
    );
};

export default Home;