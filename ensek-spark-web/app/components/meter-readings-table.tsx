import MeterReading from "../models/meter-reading.model";

import styles from './meter-readings-table.module.css';

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

const MeterReadingsTable = ({ readings }: { readings: MeterReading[] }) => {
    if (readings.length === 0) {
        return <p>No meter readings found.</p>;
    }

    return (
        <div className={styles["readings-section"]}>
            <table>
                <thead>
                    <tr>
                        <th>Account ID</th>
                        <th>Timestamp</th>
                        <th>Value</th>
                    </tr>
                </thead>
                <tbody>
                    {readings.map((reading, index) => (
                        <tr key={index}>
                            <td>{reading.accountId}</td>
                            <td>{formatDate(reading.meterReadingDateTime)}</td>
                            <td>{reading.meterReadValue}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default MeterReadingsTable;
