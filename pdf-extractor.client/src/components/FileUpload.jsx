import React, { useState, useEffect } from 'react';

function FileUpload() {
    const [selectedFile, setSelectedFile] = useState(null);
    const [metadataList, setMetadataList] = useState([]);

    useEffect(() => {
        console.log("calling metadata")
        fetch('https://localhost:32775/api/pdf/metadata')
            .then(res => res.json())
            .then(data => setMetadataList(data));
    }, []);

    const handleFileChange = (e) => {
        setSelectedFile(e.target.files[0]);
    };

    const handleUpload = async () => {
        if (!selectedFile || selectedFile.type !== 'application/pdf') {
            alert("Only PDF files are allowed.");
            return;
        }

        const formData = new FormData();
        formData.append('formFile', selectedFile);
        console.log("calling Upload")
        const response = await fetch('https://localhost:32775/api/pdf/upload', {
            method: 'POST',           
            body: formData,
        });

        console.log(response);
        if (response.ok) {
            const newMeta = await response.json();
            setMetadataList(prev => [...prev, newMeta]);
        }
    };

    return (
        <div>
            <h2>Upload PDF</h2>
            <input type="file" accept="application/pdf" onChange={handleFileChange} />
            <button onClick={handleUpload}>Upload</button>

            <h3>Metadata</h3>
            <ul>
                {metadataList.map(meta => (
                    <li key={meta.id}>
                        <strong>{meta.fileName}</strong> - {meta.title || 'N/A'} by {meta.author || 'Unknown'} - {meta.pageCount} pages
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default FileUpload;