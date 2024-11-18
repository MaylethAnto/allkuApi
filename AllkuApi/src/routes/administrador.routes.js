const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// Get all administradores
router.get('/', async (req, res) => {
    try {
        const administradoresSnapshot = await db.collection('administradores').get();
        const administradores = [];
        administradoresSnapshot.forEach(doc => {
            administradores.push({ id: doc.id, ...doc.data() });
        });
        res.json(administradores);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Get administrador by id
router.get('/:id', async (req, res) => {
    try {
        const administradorDoc = await db.collection('administradores').doc(req.params.id).get();
        if (!administradorDoc.exists) {
            return res.status(404).json({ message: 'Administrador no encontrado' });
        }
        res.json({ id: administradorDoc.id, ...administradorDoc.data() });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Create administrador
router.post('/', async (req, res) => {
    try {
        const { id_administrador, ...administradorData } = req.body;
        await db.collection('administradores').doc(id_administrador).set(administradorData);
        res.status(201).json({ id: id_administrador, ...administradorData });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Update administrador
router.put('/:id', async (req, res) => {
    try {
        const administradorRef = db.collection('administradores').doc(req.params.id);
        const administrador = await administradorRef.get();
        if (!administrador.exists) {
            return res.status(404).json({ message: 'Administrador no encontrado' });
        }
        await administradorRef.update(req.body);
        res.json({ message: 'Administrador actualizado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Delete administrador
router.delete('/:id', async (req, res) => {
    try {
        const administradorRef = db.collection('administradores').doc(req.params.id);
        const administrador = await administradorRef.get();
        if (!administrador.exists) {
            return res.status(404).json({ message: 'Administrador no encontrado' });
        }
        await administradorRef.delete();
        res.json({ message: 'Administrador eliminado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

module.exports = router;
