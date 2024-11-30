import { useState } from "react";
import { useFileUploader, useMeterReadings } from "../hooks/custom-hooks";

import FileUploader from "./file-uploader";
import MeterReadingsTable from "./meter-readings-table";

const Home = () => {
    const { readings, error } = useMeterReadings();
    const { uploadMessage, failedDetails, handleUpload } = useFileUploader();
    const [isExpanded, setIsExpanded] = useState<boolean>(false);

    return (
        <div className="container">
            <h1>ENSEK Spark</h1>
            <h2>Meter Readings</h2>

            <FileUploader onUpload={handleUpload} />
            {uploadMessage && <p className="message">{uploadMessage}</p>}

            {failedDetails && failedDetails.length > 0 && (
                <div className="collapsible-section">
                    <button
                        className="toggle-button"
                        onClick={() => setIsExpanded(!isExpanded)}
                    >
                        {isExpanded ? "Hide Failed Details" : "Show Failed Details"}
                    </button>
                    {isExpanded && (
                        <ul className="error-list">
                            {failedDetails.map((detail, index) => (
                                <li key={index}>{detail}</li>
                            ))}
                        </ul>
                    )}
                </div>
            )}

            <div className="readings-section">
                {error && <p className="error">{error}</p>}
                <MeterReadingsTable readings={readings} />
            </div>
        </div>
    );
};

export default Home;