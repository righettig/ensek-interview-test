import { useFileUploader, useMeterReadings } from "../hooks/custom-hooks";

import FileUploader from "./file-uploader";
import MeterReadingsList from "./meter-readings-list";

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