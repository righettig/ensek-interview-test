import MeterReading from "../models/meter-reading.model";

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

export default MeterReadingsList;