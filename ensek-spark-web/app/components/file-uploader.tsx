import { useState } from "react";

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

export default FileUploader;